namespace SchoolManagement.Core.DTOs.Response;

public class ExcuseDetailResponseDto
{
    public string ExcuseId { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string? ExamId { get; set; }
    public string ExcuseDescription { get; set; } = string.Empty;
    public DateTime RequestDate { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? DocumentPath { get; set; }
    public string Status { get; set; } = string.Empty;
}