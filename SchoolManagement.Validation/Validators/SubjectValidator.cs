using SchoolManagement.Validation.Models;
using SchoolManagement.Core.DTOs.Request;

namespace SchoolManagement.Validation.Validators;

public static class SubjectValidator
{
    public static ValidationResult ValidateCreate(SubjectCreateDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.SubjectId))
        {
            result.AddError("Subject ID is required");
        }

        if (string.IsNullOrWhiteSpace(request.SubjectName))
        {
            result.AddError("Subject name is required");
        }
        else if (request.SubjectName.Length < 3)
        {
            result.AddError("Subject name must be at least 3 characters");
        }

        return result;
    }
}