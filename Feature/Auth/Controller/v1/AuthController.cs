namespace AspNetEmptyApp.Feature.Auth.Controller.v1;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Asp.Versioning;
using AspNetEmptyApp.Feature.Auth.DTOs;
using AspNetEmptyApp.Feature.Auth.Handler;
using AspNetEmptyApp.Feature.Auth.Model;
using AspNetEmptyApp.Feature.Auth.Query;
using AspNetEmptyApp.Feature.Auth.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly GetUserProfileHandler _getUserProfileHandler;
    private readonly IValidator<LoginDto> _loginValidator;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenService tokenService,
        GetUserProfileHandler getUserProfileHandler,
        IValidator<LoginDto> loginValidator)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _getUserProfileHandler = getUserProfileHandler;
        _loginValidator = loginValidator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var validationResult = await _loginValidator.ValidateAsync(dto);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.ToDictionary());
        }

        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        var (token, expiresAt, roles, permissions) = await _tokenService.GenerateTokenAsync(user);

        return Ok(new AuthResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Roles = roles,
            Permissions = permissions
        });
    }

    [Authorize]
    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                     ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            return Unauthorized(new { message = "Invalid user token credentials" });
        }

        var profile = await _getUserProfileHandler.HandleAsync(new GetUserProfileQuery(userId));
        if (profile == null)
        {
            return NotFound(new { message = "User profile not found" });
        }

        return Ok(profile);
    }
}
