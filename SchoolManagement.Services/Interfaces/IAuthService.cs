using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    string GenerateJwtToken(string userId, string email, string role, string name);
}