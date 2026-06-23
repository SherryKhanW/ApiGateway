using ApiGateway.Authentication;
using ApiGateway.Models.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ApiGateway.Controllers;

[ApiController]
[Route("auth")]
[Authorize(Policy = "AuthenticatedUser")]

public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IJwtService _jwtService;

    public AuthController(IAuthService authService, IJwtService jwtService)
    {
        _authService = authService;
        _jwtService = jwtService;
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var token = await _authService.LoginAsync(request);

            return Ok(new LoginResponse
            {
                Token = token
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized();
        }
    }

    [HttpGet("verify")]
    public IActionResult Verify()
    {
        return Ok(true);
    }
    
    [HttpPut("country")]
    public async Task<IActionResult> ChangeCountry([FromBody] ChangeCountryRequest request)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrWhiteSpace(userIdClaim))
        {
            return Unauthorized();
        }

        if (!int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        try
        {
            var user = await _authService.ChangeCountryAsync(userId, request);
            return Ok(user);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    
    [Authorize]
    [HttpPut("password")]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _authService.ChangePasswordAsync(userId, request);

        if (!result)
            return Unauthorized("Old password is incorrect.");

        return Ok(true);
    }
}