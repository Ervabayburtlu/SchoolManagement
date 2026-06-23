using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize] // Sunum/test sırasında yetki takılması yaşamamak için istersen şimdilik yorumda kalabilir
public class ExamPredictionController : ControllerBase
{
    private readonly IPredictionService _predictionService;

    public ExamPredictionController(IPredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpPost("predict")]
    public async Task<IActionResult> PredictAttendance([FromBody] ExamPredictionRequestDto request)
    {
        var result = await _predictionService.PredictExamAttendanceAsync(request);

        if (!result.Success)
        {
            // Senin ApiResponse standartlarına uygun Hata formatı
            return BadRequest(ApiResponse<object>.ErrorResponse(
                result.Error ?? "Tahminleme sırasında hata oluştu."));
        }

        // Vue.js tarafına dönecek temiz nesne
        var responseData = new
        {
            SubjectId = request.SubjectId,
            SubjectName = request.SubjectName,
            ExamType = request.ExamType,
            PredictedAttendance = result.PredictedValue,
            RegisteredStudentCount = result.RegisteredStudentCount // DÜZELTME BURADA!
        };

        // Senin ApiResponse standartlarına uygun Başarı formatı
        return Ok(ApiResponse<object>.SuccessResponse(responseData, "Sınav katılım tahmini başarıyla hesaplandı."));
    }
}