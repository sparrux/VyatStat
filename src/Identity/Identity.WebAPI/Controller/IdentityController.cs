using System.Security.Claims;
using Identity.WebAPI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using OpenIddict.Validation.AspNetCore;

namespace Identity.WebAPI.Controller;

[ApiController]
public class IdentityController(
    UserManager<IdentityUser<Guid>> userManager
) : ControllerBase
{
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    [HttpGet("/profile")]
    public async Task<ActionResult<ProfileResponse>> GetProfileInfo()
    {
        if (User.Identity is null || !User.Identity.IsAuthenticated)
            return Unauthorized();
        
        var username = User.FindFirst("username")?.Value;
        var user = await userManager.FindByNameAsync(username!);

        if (user is null)
            return NotFound();

        return Ok(new ProfileResponse(user.Id, user.UserName));
    }
    
    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegistrationRequest request)
    {
        var user = new IdentityUser<Guid>
        {
            UserName = request.Login,
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }
            return BadRequest(ModelState);
        }

        return Ok(new { Message = "Пользователь успешно зарегистрирован." });
    }
}