using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class StudentSubjectRepository : GenericRepository<StudentSubject>, IStudentSubjectRepository
{
    public StudentSubjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<StudentSubject?> GetByStudentAndSubjectAsync(string studentNo, string subjectId)
    {
        return await _dbSet
            .Include(ss => ss.Student)
            .Include(ss => ss.Subject)
            .ThenInclude(s => s.Academician)
            .FirstOrDefaultAsync(ss => ss.StudentNo == studentNo && ss.SubjectId == subjectId);
    }

    public async Task<IEnumerable<StudentSubject>> GetByStudentAsync(string studentNo)
    {
        return await _dbSet
            .Where(ss => ss.StudentNo == studentNo)
            .Include(ss => ss.Subject)
            .ThenInclude(s => s.Academician)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentSubject>> GetBySubjectAsync(string subjectId)
    {
        return await _dbSet
            .Where(ss => ss.SubjectId == subjectId)
            .Include(ss => ss.Student)
            .ToListAsync();
    }

    public async Task<int> GetRegisteredStudentCountAsync(string subjectId)
    {
        return await _dbSet.CountAsync(ss => ss.SubjectId == subjectId);
    }
}