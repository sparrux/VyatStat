using System.Net.Mime;
using FluentResults.Extensions.AspNetCore;
using Identity.WebAPI.Authentication;
using Identity.WebAPI.Contracts;
using Identity.WebAPI.Exceptions;
using Identity.WebAPI.Services.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity.WebAPI.Controller;

public sealed class AccountController(
    UserManager<IdentityUser<Guid>> userManager,
    SignInManager<IdentityUser<Guid>> signInManager,
    IReturnUrlValidator returnUrlValidator,
    IUsersService usersService
) : IdentityControllerBase
{
    [AllowAnonymous]
    [HttpPost("/register")]
    public async Task<IActionResult> Register(RegistrationRequest request)
    {
        var result = await usersService.CreateAsync(request);
        return result.ToActionResult();
    }

    [AllowAnonymous]
    [HttpGet("/account/session")]
    public async Task<IActionResult> GetSession()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
        if (!result.Succeeded || result.Principal is null)
            return Unauthorized();

        var user = await userManager.GetUserAsync(result.Principal);
        if (user is null || await userManager.IsLockedOutAsync(user))
            return Unauthorized();

        return Ok(new AccountActionResponse(true));
    }

    [AllowAnonymous]
    [HttpPost("/account/login")]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, [FromQuery] string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(request.Login) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new AccountActionResponse(false));

        var user = await userManager.FindByNameAsync(request.Login.Trim());
        if (user is null)
            return Unauthorized(new { error = ApiErrors.OAuth.InvalidUserCredentials });

        if (await userManager.IsLockedOutAsync(user))
            return Unauthorized(new { error = ApiErrors.OAuth.AccountLockedOut });

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
            return Unauthorized(new { error = ApiErrors.OAuth.InvalidUserCredentials });

        await signInManager.SignInAsync(user, isPersistent: true);

        if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrlValidator.IsValidAuthorizeReturnUrl(returnUrl))
            return Redirect(returnUrl);

        return Ok(new AccountActionResponse(true));
    }

    [AllowAnonymous]
    [HttpPost("/account/logout")]
    public async Task<IActionResult> Logout([FromQuery] string? returnUrl)
    {
        await signInManager.SignOutAsync();

        if (!string.IsNullOrWhiteSpace(returnUrl) && returnUrlValidator.IsValidClientReturnUrl(returnUrl))
            return Redirect(returnUrl);

        return Ok(new AccountActionResponse(true));
    }

    [AllowAnonymous]
    [HttpGet("/account/logout")]
    public Task<IActionResult> LogoutGet([FromQuery] string? returnUrl) =>
        Logout(returnUrl);
}
