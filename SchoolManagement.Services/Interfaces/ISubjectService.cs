using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface ISubjectService
{
    Task<SubjectResponseDto?> GetByIdAsync(string subjectId);
    Task<IEnumerable<SubjectResponseDto>> GetAllAsync();
    Task<IEnumerable<SubjectResponseDto>> GetByAcademicianAsync(string academicianId);
    Task<IEnumerable<SubjectResponseDto>> GetByStudentAsync(string studentNo);
    Task<SubjectResponseDto> CreateAsync(SubjectCreateDto request);
    Task<bool> DeleteAsync(string subjectId);
    Task<bool> ExistsAsync(string subjectId);
}