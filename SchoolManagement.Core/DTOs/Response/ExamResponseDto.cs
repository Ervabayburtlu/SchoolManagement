namespace SchoolManagement.Core.DTOs.Response;

public class ExamResponseDto
{
    public string ExamId { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string? ExamDescription { get; set; }

    public string ParticipationStatus { get; set; } = "PENDING";
    public string ParticipationNotification { get; set; } = string.Empty;
}