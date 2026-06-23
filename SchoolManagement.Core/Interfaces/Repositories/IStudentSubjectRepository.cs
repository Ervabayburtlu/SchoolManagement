using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IStudentSubjectRepository : IGenericRepository<StudentSubject>
{
    Task<StudentSubject?> GetByStudentAndSubjectAsync(string studentNo, string subjectId);
    Task<IEnumerable<StudentSubject>> GetByStudentAsync(string studentNo);
    Task<IEnumerable<StudentSubject>> GetBySubjectAsync(string subjectId);
    Task<int> GetRegisteredStudentCountAsync(string subjectId);
}