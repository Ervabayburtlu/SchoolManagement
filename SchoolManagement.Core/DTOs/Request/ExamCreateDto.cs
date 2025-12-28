namespace SchoolManagement.Core.DTOs.Request;

public class ExamCreateDto
{
    public string ExamId { get; set; } = string.Empty;
    public string SubjectId { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
    public DateTime ExamDate { get; set; }
    public string? ExamDescription { get; set; }
}