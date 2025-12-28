using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;

namespace SchoolManagement.Infrastructure.Repositories;

public class StudentExamRepository : GenericRepository<StudentExam>, IStudentExamRepository
{
    public StudentExamRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<StudentExam?> GetByStudentAndExamAsync(string studentNo, string examId)
    {
        return await _dbSet
            .Include(se => se.Student)
            .Include(se => se.Exam)
            .ThenInclude(e => e.Subject)
            .FirstOrDefaultAsync(se => se.StudentNo == studentNo && se.ExamId == examId);
    }

    public async Task<IEnumerable<StudentExam>> GetByStudentAsync(string studentNo)
    {
        return await _dbSet
            .Where(se => se.StudentNo == studentNo)
            .Include(se => se.Exam)
            .ThenInclude(e => e.Subject)
            .OrderByDescending(se => se.Exam.ExamDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<StudentExam>> GetByExamAsync(string examId)
    {
        return await _dbSet
            .Where(se => se.ExamId == examId)
            .Include(se => se.Student)
            .ToListAsync();
    }
}