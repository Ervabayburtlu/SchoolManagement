using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IAdvisorRepository : IGenericRepository<Advisor>
{
    Task<Advisor?> GetByEmailAsync(string email);
    Task<Advisor?> GetByIdWithStudentsAsync(string advisorId);
    Task<bool> IsEmailUniqueAsync(string email, string? excludeAdvisorId = null);
}