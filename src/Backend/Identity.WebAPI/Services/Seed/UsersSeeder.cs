using System.Diagnostics;
using Identity.WebAPI.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Identity.WebAPI.Services.Seed;

static class UsersSeeder
{
    static readonly Guid PrimaryUserId = Guid.Parse("e9dd9922-ca57-46e1-b122-1793e2be260d");
    
    public static async Task SeedAsync(WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser<Guid>>>();

        var exists = await userManager.FindByIdAsync(PrimaryUserId.ToString());
        
        if (exists is not null)
            return;

        var user = new IdentityUser<Guid>
        {
            Id = PrimaryUserId,
            UserName = "primary"
        };

        var creation = await userManager.CreateAsync(user, "asd1234");

        var permissions = await userManager.AddClaimsAsync(user, 
        [
            new(UserClaimTypes.Role, UserClaims.Admin),
            new(UserClaimTypes.Permission, UserClaims.CanReadUsers),
            new(UserClaimTypes.Permission, UserClaims.CanUpdateUserPermissions),
        ]);
        
        Debug.Print("Primary user initialized");
    }
}