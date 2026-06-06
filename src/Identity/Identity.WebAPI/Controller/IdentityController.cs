using System.Security.Claims;
using Identity.WebAPI.Contracts;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;

namespace Identity.WebAPI.Controller;

[ApiController]
public class IdentityController(
    UserManager<IdentityUser<Guid>> userManager,
    SignInManager<IdentityUser<Guid>> signInManager
) : ControllerBase
{
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