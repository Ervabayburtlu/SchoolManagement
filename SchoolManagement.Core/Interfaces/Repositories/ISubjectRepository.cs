using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface ISubjectRepository : IGenericRepository<Subject>
{
    Task<Subject?> GetByIdWithDetailsAsync(string subjectId);
    Task<IEnumerable<Subject>> GetSubjectsByAcademicianAsync(string academicianId);
    Task<IEnumerable<Subject>> GetSubjectsByStudentAsync(string studentNo);
}