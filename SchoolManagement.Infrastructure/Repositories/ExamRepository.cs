using Microsoft.EntityFrameworkCore;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Data;
using SchoolManagement.Core.DTOs;
using System.Linq;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Infrastructure.Repositories;

public class ExamRepository(ApplicationDbContext context) : GenericRepository<Exam>(context), IExamRepository
{
    // --- 1. Mevcut (Eski) Metodların Implementasyonu ---

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
        return await _dbSet.AnyAsync(e => e.ExamDate == examDate);
    }

    // --- 2. Yeni Eklenen (E-posta/Job) Metodların Implementasyonu ---

    public async Task<List<ExamReminderDto>> GetStudentsWithoutNotificationAsync(DateTime start, DateTime end)
    {
        // _context.Set<StudentExam>() üzerinden Queryable olarak işlem yapıyoruz
        return await _context.Set<StudentExam>()
            .Include(se => se.Student)
            .Include(se => se.Exam)
                .ThenInclude(e => e.Subject)
            .Where(se => se.Exam.ExamDate >= start && se.Exam.ExamDate <= end 
                         && !se.ReminderEmailSent)
            .Select(se => new ExamReminderDto
            {
                StudentNoExamId = se.StudentNoExamId, 
                Email = se.Student.StudentMail, 
                FullName = se.Student.NameSurname,
                ExamName = se.Exam.Subject.SubjectName,
                ExamDate = se.Exam.ExamDate
            })
            .ToListAsync();
    }

    public async Task MarkReminderSentAsync(int studentExamId)
    {
        var record = await _context.Set<StudentExam>()
            .FirstOrDefaultAsync(se => se.StudentNoExamId == studentExamId);
        
        if (record != null)
        {
            record.ReminderEmailSent = true;
            await _context.SaveChangesAsync();
        }
    }
}