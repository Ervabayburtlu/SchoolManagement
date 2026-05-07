namespace SchoolManagement.Core.DTOs.Request;

public class ExamParticipationUpdateDto
{
    public string StudentNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}