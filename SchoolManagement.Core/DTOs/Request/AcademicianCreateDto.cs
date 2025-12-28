namespace SchoolManagement.Core.DTOs.Request;

public class AcademicianCreateDto
{
    public string AcademicianId { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AcademicianEmail { get; set; } = string.Empty;
    public string AcademicianPhone { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}