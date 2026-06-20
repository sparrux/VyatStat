using System.Security.Claims;
using FluentResults;
using Identity.WebAPI.Authentication;
using Identity.WebAPI.Authentication.Cache;
using Identity.WebAPI.Contracts;
using Identity.WebAPI.Exceptions;
using Identity.WebAPI.Extensions;
using Identity.WebAPI.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Identity.WebAPI.Services.Users;

sealed class UsersService(
    ILogger<UsersService> logger,
    UserManager<IdentityUser<Guid>> userManager,
    ApplicationDbContext dbContext,
    IMemoryCache memoryCache
) : IUsersService
{
    public async Task<Result<UsersResponse>> GetUsersAsync(int take, int skip)
    {
        take = Math.Clamp(take, 1, 100);
        skip = Math.Max(skip, 0);
        
        var usersQuery = userManager.Users
            .OrderBy(x => x.UserName)
            .Select(user => new
            {
                user.Id,
                user.UserName,
                user.Email,
                user.LockoutEnabled,
                user.LockoutEnd
            })
            .Skip(skip)
            .Take(take);

        var selection = await usersQuery.ToListAsync();
        var totalCount = await userManager.Users.CountAsync();

        var claimsByUserId = await LoadRoleAndPermissionClaimsAsync(selection.Select(u => u.Id).ToList());

        return Result.Ok(new UsersResponse(
            Users: selection.Select(user =>
                new UserResponse(
                    user.Id,
                    user.UserName,
                    user.Email,
                    claimsByUserId.GetValueOrDefault(user.Id) ?? MapClaimsToResponse([]),
                    IsUserLockedOut(user.LockoutEnabled, user.LockoutEnd))).ToList(),
            Total: totalCount));
    }

    public async Task<Result<UserResponse>> CreateAsync(RegistrationRequest request)
    {
        var user = new IdentityUser<Guid>
        {
            UserName = request.Login,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
            return Result.Ok(new UserResponse(
                user.Id, user.UserName, user.Email, MapClaimsToResponse([]), false));

        return Result.Fail<UserResponse>(
            result.Errors.Stringify());
    }

    public async Task<Result<UserResponse>> GetUserAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Fail(ApiErrors.UserNotFound);

        var claims = await GetUserClaimsInternalAsync(userId);
        var isLockedOut = await userManager.IsLockedOutAsync(user);
        return Result.Ok(new UserResponse(user.Id, user.UserName, user.Email, claims, isLockedOut));
    }

    public async Task<Result<UserClaimsResponse>> GetUserClaimsAsync(Guid userId)
    {
        if (!await userManager.Users.AnyAsync(u => u.Id == userId))
            return Result.Fail<UserClaimsResponse>(ApiErrors.UserNotFound);

        return Result.Ok(await GetUserClaimsInternalAsync(userId));
    }

    public async Task<Result<UserClaimsResponse>> UpdateUserPermissionsAsync(Guid userId, UpdateUserPermissionsRequest request)
    {
        if (await userManager.FindByIdAsync(userId.ToString()) is var user && user is null)
            return Result.Fail<UserClaimsResponse>(ApiErrors.UserNotFound);

        var claims = await userManager.GetClaimsAsync(user);
        var claimsMap = claims
            .GroupBy(x => x.Value)
            .ToDictionary(x => x.Key, x => x.ToArray());

        var updateValuesMap = new Dictionary<string, bool?>
            {
                { UserClaims.CanReadUsers, request.ReadUsers },
                { UserClaims.CanUpdateUserPermissions, request.UpdateUserPermissions },
                { UserClaims.CanLockOutUsers, request.LockOutUsers },
            }
            .Where(x => x.Value is not null)
            .ToDictionary(x => x.Key, x => x.Value);

        if (updateValuesMap.Count == 0)
            return await GetUserClaimsAsync(user.Id);

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        try
        {
            var permissionsChanged = false;

            foreach (var updatePair in updateValuesMap)
            {
                if (updatePair.Value is true && !claimsMap.ContainsKey(updatePair.Key))
                {
                    await AddClaim(user, CreateClaim(UserClaimTypes.Permission, updatePair.Key));
                    permissionsChanged = true;
                }

                if (updatePair.Value is false && claimsMap.TryGetValue(updatePair.Key, out var userClaim))
                {
                    await RemoveClaims(user, userClaim);
                    permissionsChanged = true;
                }
            }

            if (permissionsChanged)
                await UpdateSecurityStampAsync(user);

            await transaction.CommitAsync();

            if (permissionsChanged)
                SecurityStampCache.Invalidate(memoryCache, user.Id);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Failed to update permissions for user {UserId}", userId);
            return Result.Fail<UserClaimsResponse>(ApiErrors.FailedToUpdatePermissions);
        }

        return await GetUserClaimsAsync(user.Id);
    }

    public async Task<Result> SetLockOutAsync(Guid userId, bool isLocked)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Fail(ApiErrors.UserNotFound);

        IdentityResult result;

        if (isLocked)
        {
            result = await userManager.SetLockoutEnabledAsync(user, true);
            if (!result.Succeeded)
                return Result.Fail(result.Errors.Stringify());

            result = await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
        }
        else
        {
            result = await userManager.SetLockoutEndDateAsync(user, null);
        }

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Stringify());

        await InvalidateUserSecurityStampAsync(user);
        return Result.Ok();
    }

    public async Task<Result> UpdatePasswordAsync(Guid userId, UpdatePasswordRequest request)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        if (user is null)
            return Result.Fail(ApiErrors.UserNotFound);

        var result = await userManager.ChangePasswordAsync(
            user, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
            return Result.Fail(result.Errors.Stringify());

        await InvalidateUserSecurityStampAsync(user);
        return Result.Ok();
    }

    async Task AddClaim(IdentityUser<Guid> user, Claim claim)
    {
        var result = await userManager.AddClaimAsync(user, claim);

        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to set a claim to the user");
    }
    
    async Task RemoveClaims(IdentityUser<Guid> user, params Claim[] claims)
    {
        var result = await userManager.RemoveClaimsAsync(user, claims);
        
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to remove a claim from the user");
    }

    async Task<UserClaimsResponse> GetUserClaimsInternalAsync(Guid userId)
    {
        var claimsByUserId = await LoadRoleAndPermissionClaimsAsync([userId]);
        return claimsByUserId.GetValueOrDefault(userId) ?? MapClaimsToResponse([]);
    }

    async Task<Dictionary<Guid, UserClaimsResponse>> LoadRoleAndPermissionClaimsAsync(IReadOnlyList<Guid> userIds)
    {
        if (userIds.Count == 0)
            return [];

        var claimRows = await dbContext.Set<IdentityUserClaim<Guid>>()
            .AsNoTracking()
            .Where(c => userIds.Contains(c.UserId)
                && (c.ClaimType == UserClaimTypes.Role
                    || c.ClaimType == UserClaimTypes.Permission))
            .Select(c => new { c.UserId, c.ClaimType, c.ClaimValue })
            .ToListAsync();

        return claimRows
            .GroupBy(c => c.UserId)
            .ToDictionary(
                g => g.Key,
                g => MapClaimsToResponse(g.Select(c => new Claim(c.ClaimType!, c.ClaimValue!))));
    }

    static UserClaimsResponse MapClaimsToResponse(IEnumerable<Claim> claims)
    {
        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims.ToList()));

        return new UserClaimsResponse(
            IsAdmin: principal.HasClaim(UserClaimTypes.Role, UserClaims.Admin),
            ReadUsers: principal.HasClaim(UserClaimTypes.Permission, UserClaims.CanReadUsers),
            UpdateUserPermissions: principal.HasClaim(UserClaimTypes.Permission, UserClaims.CanUpdateUserPermissions),
            LockOutUsers: principal.HasClaim(UserClaimTypes.Permission, UserClaims.CanLockOutUsers));
    }

    static Claim CreateClaim(string type, string value) => new(type, value);

    static bool IsUserLockedOut(bool lockoutEnabled, DateTimeOffset? lockoutEnd) =>
        lockoutEnabled && lockoutEnd is { } end && end > DateTimeOffset.UtcNow;

    async Task UpdateSecurityStampAsync(IdentityUser<Guid> user)
    {
        var result = await userManager.UpdateSecurityStampAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException("Failed to update user security stamp");
    }

    async Task InvalidateUserSecurityStampAsync(IdentityUser<Guid> user)
    {
        await UpdateSecurityStampAsync(user);
        SecurityStampCache.Invalidate(memoryCache, user.Id);
    }
}
