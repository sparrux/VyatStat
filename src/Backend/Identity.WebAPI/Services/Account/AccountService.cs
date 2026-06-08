using System.Security.Claims;
using FluentResults;
using Identity.WebAPI.Authentication;
using Identity.WebAPI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Identity.WebAPI.Services.Account;

sealed class AccountService(
    ILogger<AccountService> logger,
    IAuthorizationService authorizationService,
    UserManager<IdentityUser<Guid>> userManager
) : IAccountService
{
    public async Task<Result<ProfileResponse>> CreateAsync(RegistrationRequest request)
    {
        List<IdentityError> errors = [];
        
        var user = new IdentityUser<Guid>
        {
            UserName = request.Login,
        };

        try
        {
            var result = await userManager.CreateAsync(user, request.Password);
            
            if (result.Succeeded)
                return Result.Ok(new ProfileResponse(user.Id, user.UserName));
            
            errors.AddRange(result.Errors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Fail<ProfileResponse>(ex.Message);
        }
        
        return Result.Fail<ProfileResponse>(
            errors.Select(err => $"{err.Code}: {err.Description}"));
    }

    public async Task<Result<ProfileResponse>> GetProfileAsync(Guid userId)
    {
        try
        {
            var profile = await userManager.FindByIdAsync(userId.ToString());

            if (profile is null)
                return Result.Fail("User not found");

            return Result.Ok(new ProfileResponse(profile.Id, profile.UserName));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result<UserPermissionsResponse>> GetUserPermissionsAsync(Guid userId)
    {
        if (await userManager.FindByIdAsync(userId.ToString()) is var user && user is null)
            return Result.Fail<UserPermissionsResponse>("User not found");

        var claims = await userManager.GetClaimsAsync(user);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        return Result.Ok(new UserPermissionsResponse(
            IsAdmin: principal.HasClaim(UserClaimTypes.Role, UserClaims.Admin),
            ReadUsers: principal.HasClaim(UserClaimTypes.Permission, UserClaims.CanReadUsers),
            UpdateUserPermissions: principal.HasClaim(UserClaimTypes.Permission, UserClaims.CanUpdateUserPermissions)
        ));
    }

    public async Task<Result<UserPermissionsResponse>> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsRequest request)
    {
        if (await userManager.FindByIdAsync(userId.ToString()) is var user && user is null)
            return Result.Fail<UserPermissionsResponse>("User not found");

        var claims = await userManager.GetClaimsAsync(user);
        var claimsMap = claims.ToDictionary(x => x.Value, x => x);
        
        var updateValuesMap = new Dictionary<string, bool?>
            {
                { UserClaims.CanReadUsers, request.ReadUsers },
                { UserClaims.CanUpdateUserPermissions, request.UpdateUserPermissions },
            }
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Key, x => x.Value);

        foreach (var updatePair in updateValuesMap)
        {
            if (updatePair.Value is true && !claimsMap.ContainsKey(updatePair.Key))
                await AddClaim(user, CreateClaim(UserClaimTypes.Permission, updatePair.Key));
            if (updatePair.Value is false && claimsMap.TryGetValue(updatePair.Key, out var userClaim))
                await RemoveClaim(user, userClaim);
        }
        
        return await GetUserPermissionsAsync(user.Id);
    }

    async Task AddClaim(IdentityUser<Guid> user, Claim claim)
    {
        var result = await userManager.AddClaimAsync(user, claim);

        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to set a claim to the user");
    }
    
    async Task RemoveClaim(IdentityUser<Guid> user, Claim claim)
    {
        var result = await userManager.RemoveClaimAsync(user, claim);
        
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to remove a claim from the user");
    }

    static Claim CreateClaim(string type, string value)
    {
        return new(type, value);
    }
}