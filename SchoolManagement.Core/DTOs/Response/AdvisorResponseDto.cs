namespace SchoolManagement.Core.DTOs.Response;

public class AdvisorResponseDto
{
    public string AdvisorId { get; set; } = string.Empty;
    public string NameSurname { get; set; } = string.Empty;
    public string AdvisorMail { get; set; } = string.Empty;
    public int StudentCount { get; set; }
}