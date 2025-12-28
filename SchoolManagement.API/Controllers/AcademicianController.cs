using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Services.Interfaces;
using SchoolManagement.Validation.Validators;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AcademicianController : ControllerBase
{
    private readonly IAcademicianService _academicianService;

    public AcademicianController(IAcademicianService academicianService)
    {
        _academicianService = academicianService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var academicians = await _academicianService.GetAllAsync();
        return Ok(ApiResponse<object>.SuccessResponse(academicians));
    }

    [HttpGet("{academicianId}")]
    public async Task<IActionResult> GetById(string academicianId)
    {
        var academician = await _academicianService.GetByIdAsync(academicianId);
        
        if (academician == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Academician {academicianId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(academician));
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create([FromBody] AcademicianCreateDto request)
    {
        // Validation
        var validationResult = AcademicianValidator.ValidateCreate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Validation failed", 
                validationResult.Errors));
        }

        var academician = await _academicianService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { academicianId = academician.AcademicianId }, 
            ApiResponse<object>.SuccessResponse(academician, "Academician created successfully"));
    }

    [HttpDelete("{academicianId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(string academicianId)
    {
        var deleted = await _academicianService.DeleteAsync(academicianId);
        
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Academician {academicianId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Academician deleted successfully"));
    }
}
