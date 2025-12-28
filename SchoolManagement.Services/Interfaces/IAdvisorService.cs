using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface IAdvisorService
{
    Task<AdvisorResponseDto?> GetByIdAsync(string advisorId);
    Task<IEnumerable<AdvisorResponseDto>> GetAllAsync();
    Task<AdvisorResponseDto> CreateAsync(AdvisorCreateDto request);
    Task<bool> DeleteAsync(string advisorId);
    Task<bool> ExistsAsync(string advisorId);
}