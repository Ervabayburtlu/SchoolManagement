using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Services.Interfaces;
using SchoolManagement.Validation.Validators;
using SchoolManagement.Core.Enums;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExamController : ControllerBase
{
    private readonly IExamService _examService;

    public ExamController(IExamService examService)
    {
        _examService = examService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var exams = await _examService.GetAllAsync();
        return Ok(ApiResponse<object>.SuccessResponse(exams));
    }

    [HttpGet("{examId}")]
    public async Task<IActionResult> GetById(string examId)
    {
        var exam = await _examService.GetByIdAsync(examId);
        
        if (exam == null)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Exam {examId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(exam));
    }

    [HttpGet("subject/{subjectId}")]
    public async Task<IActionResult> GetBySubject(string subjectId)
    {
        var exams = await _examService.GetBySubjectAsync(subjectId);
        return Ok(ApiResponse<object>.SuccessResponse(exams));
    }

    [HttpGet("student/{studentNo}")]
    public async Task<IActionResult> GetByStudent(string studentNo)
    {
        var exams = await _examService.GetByStudentAsync(studentNo);
        return Ok(ApiResponse<object>.SuccessResponse(exams));
    }

    [HttpGet("upcoming")]
    public async Task<IActionResult> GetUpcoming()
    {
        var exams = await _examService.GetUpcomingExamsAsync();
        return Ok(ApiResponse<object>.SuccessResponse(exams));
    }

    [HttpPost]
    [Authorize(Roles = "ADMIN,ACADEMICIAN")]
    public async Task<IActionResult> Create([FromBody] ExamCreateDto request)
    {
        // Validation
        var validationResult = ExamValidator.ValidateCreate(request);
        if (!validationResult.IsValid)
        {
            return BadRequest(ApiResponse<object>.ErrorResponse(
                "Validation failed", 
                validationResult.Errors));
        }

        var exam = await _examService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { examId = exam.ExamId }, 
            ApiResponse<object>.SuccessResponse(exam, "Exam created successfully"));
    }

    [HttpDelete("{examId}")]
    [Authorize(Roles = "ADMIN,ACADEMICIAN")]
    public async Task<IActionResult> Delete(string examId)
    {
        var deleted = await _examService.DeleteAsync(examId);
        
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Exam {examId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Exam deleted successfully"));
    }

    [HttpPut("{examId}/status")]
    public async Task<IActionResult> UpdateStatus(string examId, [FromBody] ExamStatusUpdateDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Status) || string.IsNullOrWhiteSpace(request.StudentNo))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Status and StudentNo cannot be empty"));
        }

        var success = await _examService.UpdateStatusAsync(examId, request.StudentNo, request.Status, request.Notification);

        if (!success)
        {
            return NotFound(ApiResponse<object>.ErrorResponse("Student exam record not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Participation status updated successfully"));
    }

    [HttpPut("{examId}/participation")]
    [Authorize(Roles = "ADMIN,ACADEMICIAN")]
    public async Task<IActionResult> UpdateParticipation(string examId, [FromBody] ExamParticipationUpdateDto request)
    {
        if (!Enum.TryParse<SchoolManagement.Core.Enums.ParticipationStatus>(request.Status, ignoreCase: true, out var status))
            return BadRequest(ApiResponse<object>.ErrorResponse("Geçersiz katýlým durumu"));

        var success = await _examService.UpdateParticipationAsync(examId, request.StudentNo, status);

        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse("Öðrenci sýnav kaydý bulunamadý"));

        return Ok(ApiResponse<object>.SuccessResponse(null, "Katýlým durumu güncellendi"));
    }

    [HttpGet("{examId}/students")]
    [Authorize(Roles = "ADMIN,ACADEMICIAN")]
    public async Task<IActionResult> GetStudentsByExam(string examId)
    {
        var students = await _examService.GetStudentsByExamAsync(examId);
        return Ok(ApiResponse<object>.SuccessResponse(students));
    }
}

