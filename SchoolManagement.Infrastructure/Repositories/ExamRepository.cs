using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class ExamRepository : GenericRepository<Exam>, IExamRepository
{
    public ExamRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Exam?> GetByIdWithDetailsAsync(string examId)
    {
        return await _dbSet
            .Include(e => e.Subject)
                .ThenInclude(s => s.Academician)
            .Include(e => e.StudentExams)
                .ThenInclude(se => se.Student)
            .FirstOrDefaultAsync(e => e.ExamId == examId);
    }

    public async Task<IEnumerable<Exam>> GetExamsBySubjectAsync(string subjectId)
    {
        return await _dbSet
            .Where(e => e.SubjectId == subjectId)
            .Include(e => e.Subject)
            .OrderBy(e => e.ExamDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exam>> GetExamsByStudentAsync(string studentNo)
    {
        return await _dbSet
            .Include(e => e.Subject)
            .Include(e => e.StudentExams)
            .Where(e => e.StudentExams.Any(se => se.StudentNo == studentNo))
            .OrderBy(e => e.ExamDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exam>> GetExamsByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(e => e.ExamDate >= startDate && e.ExamDate <= endDate)
            .Include(e => e.Subject)
            .OrderBy(e => e.ExamDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Exam>> GetUpcomingExamsAsync()
    {
        var today = DateTime.Now.Date;
        return await _dbSet
            .Where(e => e.ExamDate >= today)
            .Include(e => e.Subject)
                .ThenInclude(s => s.Academician)
            .OrderBy(e => e.ExamDate)
            .ToListAsync();
    }

    public async Task<bool> HasConflictAsync(DateTime examDate)
    {
        // O tarihte HERHANGÝ BÝR dersin sýnavý varsa true döner.
        return await _dbSet.AnyAsync(e => e.ExamDate == examDate);
    }
}

