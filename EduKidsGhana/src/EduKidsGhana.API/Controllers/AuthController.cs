using EduKidsGhana.Application.DTOs.Auth;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Shared.Results;
using Microsoft.AspNetCore.Mvc;

namespace EduKidsGhana.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost("register/parent")]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> RegisterParent([FromBody] RegisterParentDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterParentAsync(dto, ct);
        if (!result.Success) return BadRequest(ApiResponse<AuthResultDto>.Fail(result.Error ?? "Registration failed."));
        return Ok(ApiResponse<AuthResultDto>.Ok(result, "Registration successful. Welcome to EduKids Ghana!"));
    }

    [HttpPost("register/admin")]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> RegisterAdmin([FromBody] RegisterAdminDto dto, CancellationToken ct)
    {
        var result = await _authService.RegisterAdminAsync(dto, ct);
        if (!result.Success) return BadRequest(ApiResponse<AuthResultDto>.Fail(result.Error ?? "Registration failed."));
        return Ok(ApiResponse<AuthResultDto>.Ok(result, "Admin registered."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _authService.LoginAsync(dto, ct);
        if (!result.Success) return Unauthorized(ApiResponse<AuthResultDto>.Fail(result.Error ?? "Invalid credentials."));
        return Ok(ApiResponse<AuthResultDto>.Ok(result, "Login successful."));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Refresh([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        var result = await _authService.RefreshTokenAsync(dto.RefreshToken, ct);
        if (!result.Success) return Unauthorized(ApiResponse<AuthResultDto>.Fail(result.Error ?? "Invalid token."));
        return Ok(ApiResponse<AuthResultDto>.Ok(result));
    }

    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        var userId = GetUserId();
        if (userId == null) return Unauthorized();
        await _authService.LogoutAsync(userId.Value, dto.RefreshToken, ct);
        return Ok(ApiResponse<object>.Ok("Logged out successfully."));
    }

    private Guid? GetUserId()
    {
        var claim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(claim, out var id) ? id : null;
    }
}
