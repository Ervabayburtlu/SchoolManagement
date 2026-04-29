namespace SchoolManagement.Core.DTOs.Response;

public class StudentResponseDto
{
    public string StudentNo { get; set; } = string.Empty;
    public string NameSurname { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public string StudentMail { get; set; } = string.Empty;
    public string? AdvisorId { get; set; }
    public string? AdvisorName { get; set; }
    public int ActiveBarCount { get; set; }
    public bool IsLocked { get; set; }
    public DateTime? LockedAt { get; set; }
}