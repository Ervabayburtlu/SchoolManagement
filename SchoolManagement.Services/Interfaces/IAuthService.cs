using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    string GenerateJwtToken(string userId, string email, string role, string name);
    Task<LoginResponseDto?> ObsLoginAsync(LoginRequestDto request); // Dönüþ tipin (LoginResponseDto) kendi projendekiyle ayný olmalý.

    // Refresh Token
    Task<LoginResponseDto?> RefreshTokenAsync(RefreshRequestDto request);
    Task LogoutAsync(string userId, string role);
}