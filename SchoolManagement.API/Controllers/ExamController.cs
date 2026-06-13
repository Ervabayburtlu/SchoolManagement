using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Services.Interfaces;
using SchoolManagement.Validation.Validators;
using SchoolManagement.Core.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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

    // JWT "sub" claim'inden giriþ yapan kullanýcýnýn ID'sini al
    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                       ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                       ?? User.FindFirst("sub")?.Value;

    private bool IsAdmin => User.IsInRole("ADMIN");

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
        // Öðrenci sadece kendi sýnav listesini görebilir; ADMIN/ACADEMICIAN/ADVISOR herkesi görebilir
        if (User.IsInRole("STUDENT") && CurrentUserId != studentNo)
        {
            return Forbid();
        }

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

        // Akademisyen sadece kendi dersi için sýnav oluþturabilir
        if (User.IsInRole("ACADEMICIAN"))
        {
            var subject = await _examService.GetSubjectForOwnershipCheckAsync(request.SubjectId);
            if (subject == null || subject.AcademicianId != CurrentUserId)
            {
                return Forbid();
            }
        }

        var exam = await _examService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { examId = exam.ExamId },
            ApiResponse<object>.SuccessResponse(exam, "Exam created successfully"));
    }

    [HttpDelete("{examId}")]
    [Authorize(Roles = "ADMIN,ACADEMICIAN")]
    public async Task<IActionResult> Delete(string examId)
    {
        if (User.IsInRole("ACADEMICIAN"))
        {
            var owns = await _examService.AcademicianOwnsExamAsync(examId, CurrentUserId!);
            if (!owns) return Forbid();
        }

        var deleted = await _examService.DeleteAsync(examId);

        if (!deleted)
        {
            return NotFound(ApiResponse<object>.ErrorResponse($"Exam {examId} not found"));
        }

        return Ok(ApiResponse<object>.SuccessResponse(null, "Exam deleted successfully"));
    }

    // Öðrencinin kendi katýlým bildirimini güncellediði endpoint
    [HttpPut("{examId}/status")]
    public async Task<IActionResult> UpdateStatus(string examId, [FromBody] ExamStatusUpdateDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Status) || string.IsNullOrWhiteSpace(request.StudentNo))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("Status and StudentNo cannot be empty"));
        }

        // Sadece kendi bildirimini güncelleyebilir (STUDENT) veya ADMIN herkes için yapabilir
        if (!IsAdmin && request.StudentNo != CurrentUserId)
        {
            return Forbid();
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

        // Akademisyen sadece kendi dersinin sýnavý için iþlem yapabilir
        if (User.IsInRole("ACADEMICIAN"))
        {
            var owns = await _examService.AcademicianOwnsExamAsync(examId, CurrentUserId!);
            if (!owns) return Forbid();
        }

        var success = await _examService.UpdateParticipationAsync(examId, request.StudentNo, status);

        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse("Öðrenci sýnav kaydý bulunamadý"));

        return Ok(ApiResponse<object>.SuccessResponse(null, "Katýlým durumu güncellendi"));
    }

    [HttpGet("{examId}/students")]
    [Authorize(Roles = "ADMIN,ACADEMICIAN")]
    public async Task<IActionResult> GetStudentsByExam(string examId)
    {
        if (User.IsInRole("ACADEMICIAN"))
        {
            var owns = await _examService.AcademicianOwnsExamAsync(examId, CurrentUserId!);
            if (!owns) return Forbid();
        }

        var students = await _examService.GetStudentsByExamAsync(examId);
        return Ok(ApiResponse<object>.SuccessResponse(students));
    }
}