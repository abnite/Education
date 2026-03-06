using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EduKidsGhana.Application.DTOs.Auth;
using EduKidsGhana.Application.Interfaces;
using EduKidsGhana.Domain.Constants;
using EduKidsGhana.Domain.Entities.Identity;
using EduKidsGhana.Domain.Entities.Users;
using EduKidsGhana.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace EduKidsGhana.Infrastructure.Services.Auth;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        AppDbContext db,
        IConfiguration config,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _db = db;
        _config = config;
        _logger = logger;
    }

    public async Task<AuthResultDto> RegisterParentAsync(RegisterParentDto dto, CancellationToken ct = default)
    {
        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            return new AuthResultDto { Success = false, Error = "An account with this email already exists." };

        var user = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return new AuthResultDto { Success = false, Error = string.Join(", ", result.Errors.Select(e => e.Description)) };

        await _userManager.AddToRoleAsync(user, DomainConstants.Roles.Parent);

        _db.ParentProfiles.Add(new ParentProfile
        {
            UserId = user.Id,
            PhoneNumber = dto.PhoneNumber,
            Region = dto.Region,
            City = dto.City
        });
        await _db.SaveChangesAsync(ct);

        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthResultDto> RegisterAdminAsync(RegisterAdminDto dto, CancellationToken ct = default)
    {
        var adminSecret = _config["AdminSettings:AdminSecret"] ?? "EduKidsAdmin2024!";
        if (dto.AdminSecret != adminSecret)
            return new AuthResultDto { Success = false, Error = "Invalid admin secret." };

        var existing = await _userManager.FindByEmailAsync(dto.Email);
        if (existing != null)
            return new AuthResultDto { Success = false, Error = "Email already registered." };

        var user = new ApplicationUser
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            UserName = dto.Email,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return new AuthResultDto { Success = false, Error = string.Join(", ", result.Errors.Select(e => e.Description)) };

        await _userManager.AddToRoleAsync(user, DomainConstants.Roles.Admin);
        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null || !user.IsActive)
            return new AuthResultDto { Success = false, Error = "Invalid email or password." };

        var valid = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!valid)
            return new AuthResultDto { Success = false, Error = "Invalid email or password." };

        return await GenerateAuthResultAsync(user);
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var token = await _db.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == refreshToken && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow, ct);

        if (token == null)
            return new AuthResultDto { Success = false, Error = "Invalid or expired refresh token." };

        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        return await GenerateAuthResultAsync(token.User);
    }

    public async Task<bool> LogoutAsync(Guid userId, string refreshToken, CancellationToken ct = default)
    {
        var token = await _db.RefreshTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Token == refreshToken, ct);
        if (token == null) return false;
        token.IsRevoked = true;
        token.RevokedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    private async Task<AuthResultDto> GenerateAuthResultAsync(ApplicationUser user)
    {
        var roles = await _userManager.GetRolesAsync(user);
        var jwt = GenerateJwt(user, roles);
        var refresh = GenerateRefreshToken();

        _db.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = refresh,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });
        await _db.SaveChangesAsync();

        return new AuthResultDto
        {
            Success = true,
            Token = jwt,
            RefreshToken = refresh,
            TokenExpiry = DateTime.UtcNow.AddHours(1),
            User = new UserInfoDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Roles = roles.ToList()
            }
        };
    }

    private string GenerateJwt(ApplicationUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:Key"] ?? "EduKidsGhana-Super-Secret-Key-2024-Production!"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
        };
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"] ?? "EduKidsGhana",
            audience: _config["JwtSettings:Audience"] ?? "EduKidsGhanaApp",
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
