using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface IExamService
{
    Task<ExamResponseDto?> GetByIdAsync(string examId);
    Task<IEnumerable<ExamResponseDto>> GetAllAsync();
    Task<IEnumerable<ExamResponseDto>> GetBySubjectAsync(string subjectId);
    Task<IEnumerable<ExamResponseDto>> GetByStudentAsync(string studentNo);
    Task<IEnumerable<ExamResponseDto>> GetUpcomingExamsAsync();
    Task<ExamResponseDto> CreateAsync(ExamCreateDto request);
    Task<bool> DeleteAsync(string examId);
    Task<bool> ExistsAsync(string examId);
    Task<bool> UpdateStatusAsync(string examId, string studentNo, string status);
}