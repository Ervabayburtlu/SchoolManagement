// SchoolManagement.Core/DTOs/ExamReminderDto.cs
namespace SchoolManagement.Core.DTOs.Response;

public class ExamReminderDto
{
    public int StudentNoExamId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string ExamName { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
}