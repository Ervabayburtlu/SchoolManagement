using SchoolManagement.Core.Entities;
using SchoolManagement.Core.DTOs;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IExamRepository : IGenericRepository<Exam>
{
    Task<Exam?> GetByIdWithDetailsAsync(string examId);
    Task<IEnumerable<Exam>> GetExamsBySubjectAsync(string subjectId);
    Task<IEnumerable<Exam>> GetExamsByStudentAsync(string studentNo);
    Task<IEnumerable<Exam>> GetExamsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Exam>> GetUpcomingExamsAsync();
    Task<bool> HasConflictAsync(DateTime examDate);
    Task<List<ExamReminderDto>> GetStudentsWithoutNotificationAsync(DateTime start, DateTime end);
    Task MarkReminderSentAsync(int studentExamId);
}