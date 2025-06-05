using System;
using System.Runtime.CompilerServices;
using System.Text;
using API.DTOs;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(SignInManager<User> signInManager, IEmailService emailService, IConfiguration config) : BaseApicontoller
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> RegisterUser(RegisterDto registerDto)
    {
        var user = new User
        {
            UserName = registerDto.Email,
            Email = registerDto.Email,
            Displayname = registerDto.DisplayName
        };

        var result = await signInManager.UserManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            await GenerateVerificationEmailAsync(user, registerDto.Email);

            return Ok();
        }


        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.Code, error.Description);
        }

        return ValidationProblem();
    }

    private async Task GenerateVerificationEmailAsync(User user, string email)
    {
        var code = await signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var subject = "Confirm Email";
        var body = $@"
            <p>Hi {user.Displayname} </p>
            <p> Please click on the link bellow and confirm your email </p>
            <a href='{config["ClientAppUrl"]}/confirm-email?userId={user.Id}&code={code}'>CONFIRM</a>
        ";
        await emailService.SendEmailAsync(email, subject, body);

        Console.WriteLine(body);
    }

    private async Task GenerateResetPasswordEmailAsync(User user, string email)
    {
        var code = await signInManager.UserManager.GeneratePasswordResetTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        var subject = "Reset Password";
        var body = $@"
            <p>Hi {user.Displayname} </p>
            <p> Please click on the link bellow to change your password </p>
            <a href='{config["ClientAppUrl"]}/reset-password?email={email}&code={code}'>CONFIRM</a>
        ";
        await emailService.SendEmailAsync(email, subject, body);

        Console.WriteLine(body);
    }

    [AllowAnonymous]
    [HttpGet("forgot-password/{email}")]
    public async Task<ActionResult> ForgotPassword(string email)
    {
        var user = await signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null) return NotFound("User with that mail does not exist");

        await GenerateResetPasswordEmailAsync(user, email);

        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("user-info")]
    public async Task<ActionResult> GetUserInfo()
    {
        if (User.Identity?.IsAuthenticated == false) return NoContent();

        var user = await signInManager.UserManager.GetUserAsync(User);

        if (user == null) return Unauthorized();

        return Ok(new
        {
            user.Displayname,
            user.Email,
            user.Id,
            user.ImageUrl
        });
    }

    [AllowAnonymous]
    [HttpGet("resendConfirmEmail")]
    public async Task<ActionResult> ResendConfirmMail(string email)
    {
        var user = await signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user == null) return NotFound("User with that mail doenst exist");

        await GenerateVerificationEmailAsync(user, email);

        return Ok();
    }

    [HttpPost("logout")]
    public async Task<ActionResult> Logout()
    {
        await signInManager.SignOutAsync();

        return NoContent();
    }

    [HttpPost("change-password")]
    public async Task<ActionResult> ChangePassword(ChangePasswordDto passwordDto)
    {
        var user = await signInManager.UserManager.GetUserAsync(User);

        if (user == null) return Unauthorized();

        var result = await signInManager.UserManager.ChangePasswordAsync(user, passwordDto.CurrentPassword, passwordDto.NewPassword);

        if (result.Succeeded) return Ok();

        return BadRequest(result.Errors.First().Description);
    }
}
