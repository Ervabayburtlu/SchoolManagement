namespace SchoolManagement.Core.DTOs.Request;

public class SubjectCreateDto
{
    public string SubjectId { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public int DayIndex { get; set; }      
    public TimeSpan? StartTime { get; set; }
    public TimeSpan? EndTime { get; set; }  
    public string? AcademicianId { get; set; }
}