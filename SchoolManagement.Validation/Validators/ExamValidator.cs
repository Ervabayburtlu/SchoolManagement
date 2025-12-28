using SchoolManagement.Validation.Models;
using SchoolManagement.Core.DTOs.Request;

namespace SchoolManagement.Validation.Validators;

public static class ExamValidator
{
    public static ValidationResult ValidateCreate(ExamCreateDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.ExamId))
        {
            result.AddError("Exam ID is required");
        }

        if (string.IsNullOrWhiteSpace(request.SubjectId))
        {
            result.AddError("Subject ID is required");
        }

        if (string.IsNullOrWhiteSpace(request.ExamType))
        {
            result.AddError("Exam type is required");
        }

        if (request.ExamDate == default)
        {
            result.AddError("Exam date is required");
        }
        else if (request.ExamDate < DateTime.Now.AddDays(-1))
        {
            result.AddError("Exam date cannot be in the past");
        }

        return result;
    }
}