using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
{
    public SubjectRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Subject?> GetByIdWithDetailsAsync(string subjectId)
    {
        return await _dbSet
            .Include(s => s.Academician)
            .Include(s => s.StudentSubjects)
            .ThenInclude(ss => ss.Student)
            .Include(s => s.Exams)
            .FirstOrDefaultAsync(s => s.SubjectId == subjectId);
    }

    public async Task<IEnumerable<Subject>> GetSubjectsByAcademicianAsync(string academicianId)
    {
        return await _dbSet
            .Where(s => s.AcademicianId == academicianId)
            .Include(s => s.Academician)
            .ToListAsync();
    }

    public async Task<IEnumerable<Subject>> GetSubjectsByStudentAsync(string studentNo)
    {
        return await _dbSet
            .Include(s => s.Academician)
            .Include(s => s.StudentSubjects)
            .Where(s => s.StudentSubjects.Any(ss => ss.StudentNo == studentNo))
            .ToListAsync();
    }
}