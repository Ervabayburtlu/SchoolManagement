using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class StudentRepository : GenericRepository<Student>, IStudentRepository
{
    public StudentRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .Include(s => s.Advisor) 
            .FirstOrDefaultAsync(s => s.StudentMail == email);
    }

    public async Task<Student?> GetByStudentNoWithDetailsAsync(string studentNo)
    {
        return await _dbSet
            .Include(s => s.Advisor)
            .Include(s => s.StudentSubjects)
            .ThenInclude(ss => ss.Subject)
            .Include(s => s.StudentExams)
            .ThenInclude(se => se.Exam)
            .FirstOrDefaultAsync(s => s.StudentNo == studentNo);
    }

    public async Task<IEnumerable<Student>> GetStudentsByAdvisorAsync(string advisorId)
    {
        return await _dbSet
            .Where(s => s.AdvisorId == advisorId)
            .Include(s => s.Advisor)
            .ToListAsync();
    }

    public async Task<IEnumerable<Student>> GetStudentsByGradeAsync(string grade)
    {
        return await _dbSet
            .Where(s => s.Grade == grade)
            .Include(s => s.Advisor)
            .ToListAsync();
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string? excludeStudentNo = null)
    {
        var query = _dbSet.Where(s => s.StudentMail == email);
        
        if (!string.IsNullOrEmpty(excludeStudentNo))
        {
            query = query.Where(s => s.StudentNo != excludeStudentNo);
        }
        
        return !await query.AnyAsync();
    }
}