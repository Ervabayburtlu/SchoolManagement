using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class AdvisorRepository : GenericRepository<Advisor>, IAdvisorRepository
{
    public AdvisorRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Advisor?> GetByEmailAsync(string email)
    {
        return await _dbSet
            .FirstOrDefaultAsync(a => a.AdvisorMail == email);
    }

    public async Task<Advisor?> GetByIdWithStudentsAsync(string advisorId)
    {
        return await _dbSet
            .Include(a => a.Students)
            .FirstOrDefaultAsync(a => a.AdvisorId == advisorId);
    }

    public async Task<bool> IsEmailUniqueAsync(string email, string? excludeAdvisorId = null)
    {
        var query = _dbSet.Where(a => a.AdvisorMail == email);
        
        if (!string.IsNullOrEmpty(excludeAdvisorId))
        {
            query = query.Where(a => a.AdvisorId != excludeAdvisorId);
        }
        
        return !await query.AnyAsync();
    }
}