using EduKidsGhana.Application.DTOs.Auth;

namespace EduKidsGhana.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResultDto> RegisterParentAsync(RegisterParentDto dto, CancellationToken ct = default);
    Task<AuthResultDto> RegisterAdminAsync(RegisterAdminDto dto, CancellationToken ct = default);
    Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken ct = default);
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<bool> LogoutAsync(Guid userId, string refreshToken, CancellationToken ct = default);
}
