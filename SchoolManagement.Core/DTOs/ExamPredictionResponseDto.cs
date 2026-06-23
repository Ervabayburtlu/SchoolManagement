namespace SchoolManagement.Core.DTOs.Response;

public class ExamPredictionResponseDto
{
    public bool Success { get; set; }
    public int PredictedValue { get; set; }
    public string Info { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public int RegisteredStudentCount { get; set; }
}