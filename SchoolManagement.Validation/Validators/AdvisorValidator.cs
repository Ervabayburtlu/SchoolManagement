using SchoolManagement.Validation.Models;
using SchoolManagement.Validation.Validators.Common;
using SchoolManagement.Core.DTOs.Request;

namespace SchoolManagement.Validation.Validators;

public static class AdvisorValidator
{
    public static ValidationResult ValidateCreate(AdvisorCreateDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.AdvisorId))
        {
            result.AddError("Advisor ID is required");
        }

        if (string.IsNullOrWhiteSpace(request.NameSurname))
        {
            result.AddError("Name surname is required");
        }
        else if (request.NameSurname.Length < 3)
        {
            result.AddError("Name surname must be at least 3 characters");
        }

        if (string.IsNullOrWhiteSpace(request.AdvisorMail))
        {
            result.AddError("Email is required");
        }
        else if (!EmailValidator.IsValidEmail(request.AdvisorMail))
        {
            result.AddError("Invalid email format");
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