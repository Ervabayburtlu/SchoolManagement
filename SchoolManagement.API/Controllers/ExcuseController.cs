using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Services.Interfaces;
using SchoolManagement.Validation.Validators;
using System.IO;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExcuseController : ControllerBase
{
    private readonly IExcuseService _excuseService;
    private readonly IWebHostEnvironment _environment;

    public ExcuseController(IExcuseService excuseService, IWebHostEnvironment environment)
    {
        _excuseService = excuseService;
        _environment = environment;
    }

    [HttpGet("{excuseId}")]
    public async Task<IActionResult> GetById(string excuseId)
    {
        var excuse = await _excuseService.GetByIdAsync(excuseId);
        
        if (excuse == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Excuse {excuseId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(excuse));
    }

    [HttpGet("student/{studentNo}")]
    public async Task<IActionResult> GetByStudent(string studentNo)
    {
        var excuses = await _excuseService.GetByStudentAsync(studentNo);
        return Ok(ApiResponse<object>.SuccessResponse(excuses));
    }

    [HttpGet("advisor/{advisorId}")]
    [Authorize(Roles = "ADVISOR,ADMIN")]
    public async Task<IActionResult> GetByAdvisor(string advisorId)
    {
        var excuses = await _excuseService.GetByAdvisorAsync(advisorId);
        return Ok(ApiResponse<object>.SuccessResponse(excuses));
    }

    [HttpGet("pending")]
    [Authorize(Roles = "ADVISOR,ADMIN")]
    public async Task<IActionResult> GetPending()
    {
        var excuses = await _excuseService.GetPendingExcusesAsync();
        return Ok(ApiResponse<object>.SuccessResponse(excuses));
    }

    [HttpPost]
    [Authorize(Roles = "STUDENT")]
    // JSON yerine FormData kabul etmesi için [FromForm] kullanýyoruz
    public async Task<IActionResult> Create([FromForm] ExcuseCreateDto request)
    {
        // Zorunlu belge kontrolü
        if (request.Document == null || request.Document.Length == 0)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Mazeret belgesi yüklemek zorunludur."));
        }

        // Validation 
        var validationResult = ExcuseValidator.ValidateCreate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Validation failed",
                validationResult.Errors));
        }

        // Dosyayý sunucuya kaydetme iþlemi
        var uploadsFolder = Path.Combine(_environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot"), "uploads", "excuses");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var uniqueFileName = Guid.NewGuid().ToString() + "_" + request.Document.FileName;
        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await request.Document.CopyToAsync(fileStream);
        }

        // Kaydedilen dosyanýn yolunu DTO'ya ekle
        request.DocumentPath = $"/uploads/excuses/{uniqueFileName}";

        var excuse = await _excuseService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { excuseId = excuse.ExcuseId },
            ApiResponse<object>.SuccessResponse(excuse, "Excuse submitted successfully"));
    }

    [HttpPut("{excuseId}/respond")]
    [Authorize(Roles = "ADVISOR,ADMIN")]
    public async Task<IActionResult> Respond(string excuseId, [FromBody] ExcuseResponseDto request)
    {
        // Validation
        var validationResult = ExcuseValidator.ValidateResponse(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Validation failed", 
                validationResult.Errors));
        }

        try
        {
            var excuse = await _excuseService.RespondToExcuseAsync(excuseId, request);
            return Ok(ApiResponse<object>.SuccessResponse(excuse, "Excuse responded successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{excuseId}")]
    [Authorize(Roles = "ADMIN,STUDENT")]
    public async Task<IActionResult> Delete(string excuseId)
    {
        var deleted = await _excuseService.DeleteAsync(excuseId);
        
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Excuse {excuseId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Excuse deleted successfully"));
    }
}