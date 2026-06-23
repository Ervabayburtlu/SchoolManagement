namespace SchoolManagement.Core.DTOs.Request;

public class ExamPredictionRequestDto
{
    public string SubjectId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ExamType { get; set; } = string.Empty;
}