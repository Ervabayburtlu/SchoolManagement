namespace SchoolManagement.Core.DTOs.Request;

public class StudentCreateDto
{
    public string StudentNo { get; set; } = string.Empty;
    public string NameSurname { get; set; } = string.Empty;
    public string Grade { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public string StudentMail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? AdvisorId { get; set; }
}