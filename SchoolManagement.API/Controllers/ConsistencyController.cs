using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/consistency")]
[Authorize]
public class ConsistencyController : ControllerBase
{
    private readonly IConsistencyService _consistencyService;

    public ConsistencyController(IConsistencyService consistencyService)
    {
        _consistencyService = consistencyService;
    }

    // Öğrencinin bar durumunu getir (danışman veya öğrencinin kendisi görebilir)
    [HttpGet("{studentNo}")]
    public async Task<IActionResult> GetRecord(string studentNo)
    {
        var student = await _consistencyService.GetRecordAsync(studentNo);
        if (student == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Öğrenci bulunamadı"));

        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            student.ActiveBarCount,
            student.IsLocked,
            student.LockedAt
        }));
    }

    // Hesap kilidini aç (sadece danışman)
    [HttpPost("{studentNo}/unlock")]
    [Authorize(Roles = "ADVISOR")]
    public async Task<IActionResult> Unlock(string studentNo)
    {
        try
        {
            await _consistencyService.UnlockAccountAsync(studentNo);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Hesap açıldı, bar sıfırlandı."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpPatch("{studentNo}/bars")]
    [Authorize(Roles = "ADVISOR")]
    public async Task<IActionResult> SetBars(string studentNo, [FromBody] int count)
    {
        try
        {
            await _consistencyService.SetBarCountAsync(studentNo, count);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Bar sayısı güncellendi."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }
}