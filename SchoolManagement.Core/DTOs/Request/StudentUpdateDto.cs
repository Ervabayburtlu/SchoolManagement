namespace SchoolManagement.Core.DTOs.Request;

public class StudentUpdateDto
{
    public string NameSurname { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public string StudentMail { get; set; } = string.Empty;
    public string? AdvisorId { get; set; }
}