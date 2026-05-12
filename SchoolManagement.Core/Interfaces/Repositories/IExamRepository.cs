using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IExamRepository : IGenericRepository<Exam>
{
    Task<Exam?> GetByIdWithDetailsAsync(string examId);
    Task<IEnumerable<Exam>> GetExamsBySubjectAsync(string subjectId);
    Task<IEnumerable<Exam>> GetExamsByStudentAsync(string studentNo);
    Task<IEnumerable<Exam>> GetExamsByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IEnumerable<Exam>> GetUpcomingExamsAsync();
    Task<bool> HasConflictAsync(DateTime examDate);
}