// SchoolManagement.Services/Implementations/ExamReminderJob.cs
using Hangfire;
using Microsoft.Extensions.Logging;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.Interfaces.Repositories;

namespace SchoolManagement.Services.Implementations;

public class ExamReminderJob(
    IStudentExamRepository studentExamRepo,   // ✅ IExamRepository değil, bu olmalı
    IEmailService emailService,
    ILogger<ExamReminderJob> logger)
{
    [AutomaticRetry(Attempts = 3)]
    public async Task SendRemindersAsync()
    {
        var now = DateTime.Now;
        var deadline = now.AddHours(48);

        var students = await studentExamRepo.GetStudentsWithoutNotificationAsync(now, deadline);
        logger.LogInformation("{Count} öğrenciye hatırlatma gönderilecek.", students.Count);

        foreach (var s in students)
        {
            try
            {
                await emailService.SendExamReminderAsync(s.Email, s.FullName, s.ExamName, s.ExamDate);
                await studentExamRepo.MarkReminderSentAsync(s.StudentNoExamId);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "E-posta gönderilemedi: {Email}", s.Email);
            }
        }
    }
}