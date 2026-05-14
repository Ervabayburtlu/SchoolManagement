using SchoolManagement.Core.DTOs;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IStudentExamRepository : IGenericRepository<StudentExam>
{
    Task<StudentExam?> GetByStudentAndExamAsync(string studentNo, string examId);
    Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentNo);
    Task<IEnumerable<StudentExam>> GetByExamAsync(string examId);
    Task AddRangeAsync(IEnumerable<StudentExam> studentExams);
    Task<List<ExamReminderDto>> GetStudentsWithoutNotificationAsync(DateTime from, DateTime to);
    Task MarkReminderSentAsync(int studentExamId);
}