// ConsistencyController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                       ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                       ?? User.FindFirst("sub")?.Value;

    private bool IsAdmin => User.IsInRole("ADMIN");

    // Danışmanın, verilen öğrencinin kendi öğrencisi olup olmadığını kontrol eder
    private async Task<bool> AdvisorOwnsStudentAsync(string studentNo)
    {
        var student = await _consistencyService.GetRecordAsync(studentNo);
        return student != null && student.AdvisorId == CurrentUserId;
    }

    [HttpGet("{studentNo}")]
    public async Task<IActionResult> GetRecord(string studentNo)
    {
        var student = await _consistencyService.GetRecordAsync(studentNo);
        if (student == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Öğrenci bulunamadı"));

        // Öğrenci sadece kendi kaydını görebilir; danışman sadece kendi öğrencisini; admin herkesi
        if (User.IsInRole("STUDENT") && CurrentUserId != studentNo)
            return Forbid();

        if (User.IsInRole("ADVISOR") && student.AdvisorId != CurrentUserId)
            return Forbid();

        return Ok(ApiResponse<object>.SuccessResponse(new
        {
            student.ActiveBarCount,
            student.IsLocked,
            student.LockedAt,
            student.UnlockedAt
        }));
    }

    // Danışman (kendi öğrencisi) veya admin kilidi açar
    [HttpPost("{studentNo}/unlock")]
    [Authorize(Roles = "ADVISOR,ADMIN")]
    public async Task<IActionResult> Unlock(string studentNo)
    {
        if (!IsAdmin && !await AdvisorOwnsStudentAsync(studentNo))
            return Forbid();

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

    // Danışman (kendi öğrencisi) veya admin bar sayısını manuel düzenler
    [HttpPatch("{studentNo}/bars")]
    [Authorize(Roles = "ADVISOR,ADMIN")]
    public async Task<IActionResult> SetBars(string studentNo, [FromBody] int count)
    {
        if (!IsAdmin && !await AdvisorOwnsStudentAsync(studentNo))
            return Forbid();

        try
        {
            await _consistencyService.SetBarCountAsync(studentNo, count);
            return Ok(ApiResponse<object>.SuccessResponse(null, $"Bar sayısı {count} olarak güncellendi."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    // Elle girilen sınavlar için manuel tetikleme — bildirimsiz devamsızlık
    [HttpPost("{studentNo}/trigger/no-notification")]
    [Authorize(Roles = "ADVISOR,ADMIN")]
    public async Task<IActionResult> TriggerNoNotification(string studentNo)
    {
        if (!IsAdmin && !await AdvisorOwnsStudentAsync(studentNo))
            return Forbid();

        try
        {
            await _consistencyService.OnAbsentWithoutNotificationAsync(studentNo);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Bildirimsiz devamsızlık işlendi."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    // Elle girilen sınavlar için manuel tetikleme — tutarsız davranış
    [HttpPost("{studentNo}/trigger/inconsistent")]
    [Authorize(Roles = "ADVISOR,ADMIN")]
    public async Task<IActionResult> TriggerInconsistent(string studentNo)
    {
        if (!IsAdmin && !await AdvisorOwnsStudentAsync(studentNo))
            return Forbid();

        try
        {
            await _consistencyService.OnInconsistentBehaviorAsync(studentNo);
            return Ok(ApiResponse<object>.SuccessResponse(null, "Tutarsız davranış işlendi."));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }
}