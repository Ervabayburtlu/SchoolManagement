using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IStudentRepository : IGenericRepository<Student>
{
    Task<Student?> GetByEmailAsync(string email);
    Task<Student?> GetByStudentNoWithDetailsAsync(string studentNo);
    Task<IEnumerable<Student>> GetStudentsByAdvisorAsync(string advisorId);
    Task<IEnumerable<Student>> GetStudentsByGradeAsync(string grade);
    Task<bool> IsEmailUniqueAsync(string email, string? excludeStudentNo = null);
}