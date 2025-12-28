using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IStudentExamRepository : IGenericRepository<StudentExam>
{
    Task<StudentExam?> GetByStudentAndExamAsync(string studentNo, string examId);
    Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentNo);
    Task<IEnumerable<StudentExam>> GetByExamAsync(string examId);
}