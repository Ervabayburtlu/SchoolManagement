using SchoolManagement.Validation.Models;
using SchoolManagement.Validation.Validators.Common;
using SchoolManagement.Core.DTOs.Request;

namespace SchoolManagement.Validation.Validators;

public static class AcademicianValidator
{
    public static ValidationResult ValidateCreate(AcademicianCreateDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.AcademicianId))
        {
            result.AddError("Academician ID is required");
        }

        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            result.AddError("First name is required");
        }
        else if (request.FirstName.Length < 2)
        {
            result.AddError("First name must be at least 2 characters");
        }

        if (string.IsNullOrWhiteSpace(request.LastName))
        {
            result.AddError("Last name is required");
        }
        else if (request.LastName.Length < 2)
        {
            result.AddError("Last name must be at least 2 characters");
        }

        if (string.IsNullOrWhiteSpace(request.AcademicianEmail))
        {
            result.AddError("Email is required");
        }
        else if (!EmailValidator.IsValidEmail(request.AcademicianEmail))
        {
            result.AddError("Invalid email format");
        }

        if (string.IsNullOrWhiteSpace(request.AcademicianPhone))
        {
            result.AddError("Phone is required");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            result.AddError("Password is required");
        }
        else if (!PasswordValidator.IsValidPassword(request.Password))
        {
            result.AddError("Password must be at least 6 characters");
        }

        return result;
    }
}