namespace SchoolManagement.Core.DTOs.Request;

public class RefreshRequestDto
{
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}