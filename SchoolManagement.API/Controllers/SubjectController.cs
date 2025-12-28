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
public class SubjectController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var subjects = await _subjectService.GetAllAsync();
        return Ok(ApiResponse<object>.SuccessResponse(subjects));
    }

    [HttpGet("{subjectId}")]
    public async Task<IActionResult> GetById(string subjectId)
    {
        var subject = await _subjectService.GetByIdAsync(subjectId);
        
        if (subject == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Subject {subjectId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(subject));
    }

    [HttpGet("academician/{academicianId}")]
    public async Task<IActionResult> GetByAcademician(string academicianId)
    {
        var subjects = await _subjectService.GetByAcademicianAsync(academicianId);
        return Ok(ApiResponse<object>.SuccessResponse(subjects));
    }

    [HttpGet("student/{studentNo}")]
    public async Task<IActionResult> GetByStudent(string studentNo)
    {
        var subjects = await _subjectService.GetByStudentAsync(studentNo);
        return Ok(ApiResponse<object>.SuccessResponse(subjects));
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,ACADEMICIAN")]
    public async Task<IActionResult> Create([FromBody] SubjectCreateDto request)
    {
        // Validation
        var validationResult = SubjectValidator.ValidateCreate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Validation failed", 
                validationResult.Errors));
        }

        var subject = await _subjectService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { subjectId = subject.SubjectId }, 
            ApiResponse<object>.SuccessResponse(subject, "Subject created successfully"));
    }

    [HttpDelete("{subjectId}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(string subjectId)
    {
        var deleted = await _subjectService.DeleteAsync(subjectId);
        
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Subject {subjectId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Subject deleted successfully"));
    }
}

