namespace SchoolManagement.Core.DTOs.Response;

public class SubjectResponseDto
{
    public string SubjectId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string? AcademicianId { get; set; }
    public string? AcademicianName { get; set; }
}