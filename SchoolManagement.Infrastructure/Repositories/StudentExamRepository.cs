using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Core.DTOs;
using SchoolManagement.Core.Enums;

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

    public async Task AddRangeAsync(IEnumerable<StudentExam> studentExams)
    {
        await _dbSet.AddRangeAsync(studentExams);
        await _context.SaveChangesAsync();
    }

    public async Task<List<ExamReminderDto>> GetStudentsWithoutNotificationAsync(DateTime from, DateTime to)
    {
        return await _dbSet
            .Where(se =>
                se.Exam.ExamDate >= from &&
                se.Exam.ExamDate <= to &&
                se.ParticipationNotification == null &&   // bildirim yapmamış
                !se.ReminderEmailSent)                    // daha önce email gönderilmemiş
            .Include(se => se.Student)
            .Include(se => se.Exam)
            .ThenInclude(e => e.Subject)
            .Select(se => new ExamReminderDto
            {
                StudentNoExamId  = se.StudentNoExamId,
                Email         = se.Student.StudentMail,
                FullName      = se.Student.NameSurname,
                ExamName      = se.Exam.Subject.SubjectName + " - " + se.Exam.ExamType,
                ExamDate      = se.Exam.ExamDate
            })
            .ToListAsync();
    }

    public async Task MarkReminderSentAsync(int studentExamId)
    {
        var record = await _dbSet.FindAsync(studentExamId);
        if (record is not null)
        {
            record.ReminderEmailSent = true;
            await _context.SaveChangesAsync();
        }
    }
}