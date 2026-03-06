namespace EduKidsGhana.Application.DTOs.Auth;

public record LoginDto(string Email, string Password);

public record RegisterParentDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string PhoneNumber,
    string? Region,
    string? City);

public record RegisterAdminDto(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string AdminSecret);

public class AuthResultDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? TokenExpiry { get; set; }
    public UserInfoDto? User { get; set; }
    public string? Error { get; set; }
}

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public record RefreshTokenDto(string RefreshToken);
