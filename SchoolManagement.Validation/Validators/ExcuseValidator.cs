using SchoolManagement.Validation.Models;
using SchoolManagement.Core.DTOs.Request;

namespace SchoolManagement.Validation.Validators;

public static class ExcuseValidator
{
    public static ValidationResult ValidateCreate(ExcuseCreateDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.StudentNo))
        {
            result.AddError("Student number is required");
        }

        if (string.IsNullOrWhiteSpace(request.ExcuseDescription))
        {
            result.AddError("Excuse description is required");
        }
        else if (request.ExcuseDescription.Length < 10)
        {
            result.AddError("Excuse description must be at least 10 characters");
        }

        return result;
    }

    public static ValidationResult ValidateResponse(ExcuseResponseDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.ExcuseId))
        {
            result.AddError("Excuse ID is required");
        }

        if (string.IsNullOrWhiteSpace(request.Status))
        {
            result.AddError("Status is required");
        }
        else
        {
            var validStatuses = new[] { "APPROVED", "REJECTED" };
            if (!validStatuses.Contains(request.Status.ToUpper()))
            {
                result.AddError("Invalid status. Must be APPROVED or REJECTED");
            }
        }

        return result;
    }
}