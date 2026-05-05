namespace SchoolManagement.Core.DTOs.Request;
using Microsoft.AspNetCore.Http;

public class ExcuseCreateDto
{
    public string StudentNo { get; set; } = string.Empty;
    public string? ExamId { get; set; }
    public string ExcuseDescription { get; set; } = string.Empty;
    
    // Frontend'den FormData ile gelecek dosya
    public IFormFile? Document { get; set; }

    // Veritabanýna kaydedilecek dosya yolu (mevcut)
    public string? DocumentPath { get; set; }
}