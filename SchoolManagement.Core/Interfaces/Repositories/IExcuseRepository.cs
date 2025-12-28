using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IExcuseRepository : IGenericRepository<Excuse>
{
    Task<Excuse?> GetByIdWithDetailsAsync(string excuseId);
    Task<IEnumerable<Excuse>> GetExcusesByStudentAsync(string studentNo);
    Task<IEnumerable<Excuse>> GetExcusesByAdvisorAsync(string advisorId);
    Task<IEnumerable<Excuse>> GetPendingExcusesAsync();
    Task<IEnumerable<Excuse>> GetExcusesByStatusAsync(string status);
    Task<IEnumerable<Excuse>> GetExcusesByExamAsync(string examId);
}