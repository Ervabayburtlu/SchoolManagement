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
public class StudentController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var students = await _studentService.GetAllAsync();
        return Ok(ApiResponse<object>.SuccessResponse(students));
    }

    [HttpGet("{studentNo}")]
    public async Task<IActionResult> GetById(string studentNo)
    {
        var student = await _studentService.GetByIdAsync(studentNo);
        
        if (student == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Student {studentNo} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(student));
    }

    [HttpGet("advisor/{advisorId}")]
    public async Task<IActionResult> GetByAdvisor(string advisorId)
    {
        var students = await _studentService.GetByAdvisorAsync(advisorId);
        return Ok(ApiResponse<object>.SuccessResponse(students));
    }

    [HttpGet("grade/{grade}")]
    public async Task<IActionResult> GetByGrade(string grade)
    {
        var students = await _studentService.GetByGradeAsync(grade);
        return Ok(ApiResponse<object>.SuccessResponse(students));
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,ADVISOR")]
    public async Task<IActionResult> Create([FromBody] StudentCreateDto request)
    {
        // Validation
        var validationResult = StudentValidator.ValidateCreate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Validation failed", 
                validationResult.Errors));
        }

        var student = await _studentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { studentNo = student.StudentNo }, 
            ApiResponse<object>.SuccessResponse(student, "Student created successfully"));
    }

    [HttpGet("inactive")]
    [Authorize(Roles = "ADVISOR,ADMIN")]
    public async Task<IActionResult> GetInactiveStudents()
    {
        // Not: IStudentService içinde GetInactiveAsync diye bir metodun olduğunu 
        // veya eklediğini varsayıyorum.
        var students = await _studentService.GetInactiveStudentsAsync();
        return Ok(ApiResponse<object>.SuccessResponse(students));
    }
    [HttpPut("{studentNo}")]
    [Authorize(Roles = "ADMIN,ADVISOR,STUDENT")]
    public async Task<IActionResult> Update(string studentNo, [FromBody] StudentUpdateDto request)
    {
        // Validation
        var validationResult = StudentValidator.ValidateUpdate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Validation failed", 
                validationResult.Errors));
        }

        try
        {
            var student = await _studentService.UpdateAsync(studentNo, request);
            return Ok(ApiResponse<object>.SuccessResponse(student, "Student updated successfully"));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ApiResponse<object>.ErrorResponse(ex.Message));
        }
    }

    [HttpDelete("{studentNo}")]
    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Delete(string studentNo)
    {
        var deleted = await _studentService.DeleteAsync(studentNo);
        
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Student {studentNo} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Student deleted successfully"));
    }
    
}
