using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class AcademicianRepository : GenericRepository<Academician>, IAcademicianRepository
{
    public AcademicianRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Academician?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.AcademicianEmail == email);
    }

    public async Task<Academician?> GetByIdWithSubjectsAsync(string academicianId)
    {
        return await _dbSet
            .Include(a => a.Subjects)
            .FirstOrDefaultAsync(a => a.AcademicianId == academicianId);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string? excludeAcademicianId = null)
    {
        var query = _dbSet.Where(a => a.AcademicianEmail == email);
        
        if (!string.IsNullOrEmpty(excludeAcademicianId))
        {
            query = query.Where(a => a.AcademicianId != excludeAcademicianId);
        }
        
        return !await query.AnyAsync();
    }
}