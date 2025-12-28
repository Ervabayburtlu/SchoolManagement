using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface IStudentService
{
    Task<StudentResponseDto?> GetByIdAsync(string studentNo);
    Task<IEnumerable<StudentResponseDto>> GetAllAsync();
    Task<IEnumerable<StudentResponseDto>> GetByAdvisorAsync(string advisorId);
    Task<IEnumerable<StudentResponseDto>> GetByGradeAsync(string grade);
    Task<StudentResponseDto> CreateAsync(StudentCreateDto request);
    Task<StudentResponseDto> UpdateAsync(string studentNo, StudentUpdateDto request);
    Task<bool> DeleteAsync(string studentNo);
    Task<bool> ExistsAsync(string studentNo);
}