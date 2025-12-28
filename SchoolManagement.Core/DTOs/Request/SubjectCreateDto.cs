namespace SchoolManagement.Core.DTOs.Request;

public class SubjectCreateDto
{
    public string SubjectId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string? AcademicianId { get; set; }
}