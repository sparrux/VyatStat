using System.Security.Claims;
using FluentResults;
using Identity.WebAPI.Authentication;
using Identity.WebAPI.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.WebAPI.Services.Users;

sealed class UsersService(
    ILogger<UsersService> logger,
    UserManager<IdentityUser<Guid>> userManager,
    IMemoryCache memoryCache
) : IUsersService
{
    public async Task<Result<UsersResponse>> GetUsersAsync(int take, int skip)
    {
        var claims = new Dictionary<Guid, UserClaimsResponse?>();
        
        var selection = await userManager.Users
            .OrderBy(x => x.UserName)
            .Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.LockoutEnabled,
                user.LockoutEnd
            }).Skip(skip).Take(take).ToListAsync();
        
        var totalUsers = await userManager.Users.CountAsync();

        foreach (var user in selection)
        {
            var result = await GetUserClaimsAsync(user.Id);
            claims[user.Id] = result.IsSuccess ? result.Value : null;
        }
        
        return Result.Ok(new UsersResponse(
            Users: selection.Select(user =>
                new UserResponse(
                    user.Id,
                    user.UserName,
                    user.Email,
                    claims[user.Id],
                    IsUserLockedOut(user.LockoutEnabled, user.LockoutEnd))).ToList(),
            Total: totalUsers
        ));
    }

    public async Task<Result<UserResponse>> CreateAsync(RegistrationRequest request)
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
                return Result.Ok(new UserResponse(user.Id, user.UserName, user.Email, null, false));
            
            errors.AddRange(result.Errors);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Fail<UserResponse>(ex.Message);
        }
        
        return Result.Fail<UserResponse>(
            errors.Select(err => $"{err.Code}: {err.Description}"));
    }

    public async Task<Result<UserResponse>> GetUserAsync(Guid userId)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            
            var claims = await GetUserClaimsAsync(userId);
            
            if (claims.IsFailed)
                return claims.ToResult();
            
            if (user is null)
                return Result.Fail("User not found");

            var isLockedOut = await userManager.IsLockedOutAsync(user);
            return Result.Ok(new UserResponse(user.Id, user.UserName, user.Email, claims.Value, isLockedOut));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            return Result.Fail(ex.Message);
        }
    }

    public async Task<Result<UserClaimsResponse>> GetUserClaimsAsync(Guid userId)
    {
        if (await userManager.FindByIdAsync(userId.ToString()) is var user && user is null)
            return Result.Fail<UserClaimsResponse>("User not found");

        var claims = await userManager.GetClaimsAsync(user);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims));

        return Result.Ok(new UserClaimsResponse(
            IsAdmin: principal.HasClaim(UserClaimTypes.Role, UserClaims.Admin),
            ReadUsers: principal.HasClaim(UserClaimTypes.Permission, UserClaims.CanReadUsers),
            UpdateUserPermissions: principal.HasClaim(UserClaimTypes.Permission, UserClaims.CanUpdateUserPermissions),
            LockOutUsers: principal.HasClaim(UserClaimTypes.Permission, UserClaims.CanLockOutUsers)
        ));
    }

    public async Task<Result<UserClaimsResponse>> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsRequest request)
    {
        if (await userManager.FindByIdAsync(userId.ToString()) is var user && user is null)
            return Result.Fail<UserClaimsResponse>("User not found");

        var errors = new List<Error>();
        
        var claims = await userManager.GetClaimsAsync(user);
        var claimsMap = claims.ToDictionary(x => x.Value, x => x);
        
        var updateValuesMap = new Dictionary<string, bool?>
            {
                { UserClaims.CanReadUsers, request.ReadUsers },
                { UserClaims.CanUpdateUserPermissions, request.UpdateUserPermissions },
                { UserClaims.CanLockOutUsers, request.LockOutUsers },
            }
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Key, x => x.Value);

        var permissionsChanged = false;

        foreach (var updatePair in updateValuesMap)
        {
            try
            {
                if (updatePair.Value is true && !claimsMap.ContainsKey(updatePair.Key))
                {
                    await AddClaim(user, CreateClaim(UserClaimTypes.Permission, updatePair.Key));
                    permissionsChanged = true;
                }

                if (updatePair.Value is false && claimsMap.TryGetValue(updatePair.Key, out var userClaim))
                {
                    await RemoveClaim(user, userClaim);
                    permissionsChanged = true;
                }
            }
            catch (Exception ex)
            {
                errors.Add(new Error(ex.Message, new ExceptionalError(ex)));
            }
        }

        if (permissionsChanged)
            await InvalidateUserSecurityStampAsync(user);

        return (await GetUserClaimsAsync(user.Id)).WithErrors(errors);
    }

    public async Task<Result> SetLockOutAsync(Guid userId, bool isLocked)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Fail("User not found");

        IdentityResult result;

        if (isLocked)
        {
            result = await userManager.SetLockoutEnabledAsync(user, true);
            if (!result.Succeeded)
                return Result.Fail(result.Errors.Select(err => $"{err.Code}: {err.Description}"));

            result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        }
        else
        {
            result = await userManager.SetLockoutEndDateAsync(user, null);
        }

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(err => $"{err.Code}: {err.Description}"));

        await InvalidateUserSecurityStampAsync(user);
        return Result.Ok();
    }

    public async Task<Result> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Fail("User not found");

        var result = await userManager.ChangePasswordAsync(
            user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Select(err => $"{err.Code}: {err.Description}"));

        await InvalidateUserSecurityStampAsync(user);
        return Result.Ok();
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

    static Claim CreateClaim(string type, string value) => new(type, value);

    static bool IsUserLockedOut(bool lockoutEnabled, DateTimeOffset? lockoutEnd) =>
        lockoutEnabled && lockoutEnd is { } end && end > DateTimeOffset.UtcNow;

    async Task InvalidateUserSecurityStampAsync(IdentityUser<Guid> user)
    {
        var result = await userManager.UpdateSecurityStampAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to update user security stamp");

        SecurityStampCache.Invalidate(memoryCache, user.Id);
    }
}