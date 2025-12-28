namespace SchoolManagement.Core.DTOs.Request;

public class AdvisorCreateDto
{
    public string AdvisorId { get; set; } = string.Empty;
    public string NameSurname { get; set; } = string.Empty;
    public string AdvisorMail { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}