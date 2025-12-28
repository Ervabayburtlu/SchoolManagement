namespace SchoolManagement.Core.DTOs.Request;

public class ExcuseCreateDto
{
    public string StudentNo { get; set; } = string.Empty;
    public string? ExamId { get; set; }
    public string ExcuseDescription { get; set; } = string.Empty;
    public string? DocumentPath { get; set; }
}