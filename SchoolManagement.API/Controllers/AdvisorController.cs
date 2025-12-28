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
public class AdvisorController : ControllerBase
{
    private readonly IAdvisorService _advisorService;

    public AdvisorController(IAdvisorService advisorService)
    {
        _advisorService = advisorService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var advisors = await _advisorService.GetAllAsync();
        return Ok(ApiResponse<object>.SuccessResponse(advisors));
    }

    [HttpGet("{advisorId}")]
    public async Task<IActionResult> GetById(string advisorId)
    {
        var advisor = await _advisorService.GetByIdAsync(advisorId);
        
        if (advisor == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Advisor {advisorId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(advisor));
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Create([FromBody] AdvisorCreateDto request)
    {
        // Validation
        var validationResult = AdvisorValidator.ValidateCreate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Validation failed", 
                validationResult.Errors));
        }

        var advisor = await _advisorService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { advisorId = advisor.AdvisorId }, 
            ApiResponse<object>.SuccessResponse(advisor, "Advisor created successfully"));
    }

    [HttpDelete("{advisorId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(string advisorId)
    {
        var deleted = await _advisorService.DeleteAsync(advisorId);
        
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Advisor {advisorId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Advisor deleted successfully"));
    }
}