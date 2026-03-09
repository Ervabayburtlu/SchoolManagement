namespace SchoolManagement.Core.DTOs.Request;

public class LoginRequestDto
{
    private string _email = string.Empty;
    private string _role = string.Empty;

    public string Email 
    { 
        get => _email; 
        set => _email = value?.Trim() ?? string.Empty; 
    }

    public string Password { get; set; } = string.Empty;

    public string Role 
    { 
        get => _role; 
        set => _role = value?.Trim().ToUpper() ?? string.Empty; // STUDENT, ADVISOR, ACADEMICIAN
    }
}