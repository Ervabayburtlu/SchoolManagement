namespace SchoolManagement.Core.DTOs.Request;

public class ExcuseResponseDto
{
    public string ExcuseId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // APPROVED, REJECTED
}