// SchoolManagement.Core/Interfaces/IEmailService.cs
namespace SchoolManagement.Core.Interfaces.Repositories;

public interface IEmailService
{
    Task SendExamReminderAsync(string toEmail, string studentName, string examName, DateTime examDate);
}