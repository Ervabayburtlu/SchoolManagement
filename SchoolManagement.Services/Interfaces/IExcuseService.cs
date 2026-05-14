using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface IExcuseService
{
    Task<ExcuseDetailResponseDto?> GetByIdAsync(string excuseId);
    Task<IEnumerable<ExcuseDetailResponseDto>> GetByStudentAsync(string studentNo);
    Task<IEnumerable<ExcuseDetailResponseDto>> GetByAdvisorAsync(string advisorId);
    Task<IEnumerable<ExcuseDetailResponseDto>> GetPendingExcusesAsync();
    Task<ExcuseDetailResponseDto> CreateAsync(ExcuseCreateDto request);
    Task<ExcuseDetailResponseDto> RespondToExcuseAsync(string excuseId, ExcuseResponseDto request);
 
    Task<bool> DeleteAsync(string excuseId);
    
}