using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface IAcademicianService
{
    Task<AcademicianResponseDto?> GetByIdAsync(string academicianId);
    Task<IEnumerable<AcademicianResponseDto>> GetAllAsync();
    Task<AcademicianResponseDto> CreateAsync(AcademicianCreateDto request);
    Task<bool> DeleteAsync(string academicianId);
    Task<bool> ExistsAsync(string academicianId);
}