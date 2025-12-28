using SchoolManagement.Core.Entities;

namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IAcademicianRepository : IGenericRepository<Academician>
{
    Task<Academician?> GetByEmailAsync(string email);
    Task<Academician?> GetByIdWithSubjectsAsync(string academicianId);
    Task<bool> IsEmailUniqueAsync(string email, string? excludeAcademicianId = null);
}