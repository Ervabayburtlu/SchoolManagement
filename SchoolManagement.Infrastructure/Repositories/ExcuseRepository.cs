using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class ExcuseRepository : GenericRepository<Excuse>, IExcuseRepository
{
    public ExcuseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Excuse?> GetByIdWithDetailsAsync(string excuseId)
    {
        return await _dbSet
            .Include(e => e.Student)
            .Include(e => e.Advisor)
            .Include(e => e.Exam)
                .ThenInclude(ex => ex.Subject)
            .FirstOrDefaultAsync(e => e.ExcuseId == excuseId);
    }

    public async Task<IEnumerable<Excuse>> GetExcusesByStudentAsync(string studentNo)
    {
        return await _dbSet
        .Where(e => e.StudentNo == studentNo)
        .Include(e => e.Advisor)
        .Include(e => e.Exam)
            .ThenInclude(ex => ex.Subject)
        .OrderByDescending(e => e.RequestDate)
        .ToListAsync();
    }

    public async Task<IEnumerable<Excuse>> GetExcusesByAdvisorAsync(string advisorId)
    {
        return await _dbSet
            .Where(e => e.AdvisorId == advisorId)
            .Include(e => e.Student)
            .OrderByDescending(e => e.RequestDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Excuse>> GetPendingExcusesAsync()
    {
        return await _dbSet
            .Where(e => e.Status == "PENDING")
            .Include(e => e.Student)
            .Include(e => e.Advisor)
            .OrderBy(e => e.RequestDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Excuse>> GetExcusesByStatusAsync(string status)
    {
        return await _dbSet
            .Where(e => e.Status == status)
            .Include(e => e.Student)
            .Include(e => e.Advisor)
            .OrderByDescending(e => e.RequestDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Excuse>> GetExcusesByExamAsync(string examId)
    {
        return await _dbSet
            .Where(e => e.ExamId == examId)
            .Include(e => e.Student)
            .Include(e => e.Advisor)
            .OrderByDescending(e => e.RequestDate)
            .ToListAsync();
    }
}

