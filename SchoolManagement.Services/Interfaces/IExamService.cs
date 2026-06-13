using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Enums;

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
    Task<bool> UpdateStatusAsync(string examId, string studentNo, string status, string? notification = null);
    Task<IEnumerable<object>> GetStudentsByExamAsync(string examId);
    Task<bool> UpdateParticipationAsync(string examId, string studentNo, ParticipationStatus status);

    // IDOR / Ownership kontrolleri
    Task<bool> AcademicianOwnsExamAsync(string examId, string academicianId);
    Task<Subject?> GetSubjectForOwnershipCheckAsync(string subjectId);
}