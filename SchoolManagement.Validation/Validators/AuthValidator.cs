using SchoolManagement.Validation.Models;
using SchoolManagement.Validation.Validators.Common;
using SchoolManagement.Core.DTOs.Request;

namespace SchoolManagement.Validation.Validators;

public static class AuthValidator
{
    public static ValidationResult ValidateLogin(LoginRequestDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            result.AddError("Email is required");
        }
        else if (!EmailValidator.IsValidEmail(request.Email))
        {
            result.AddError("Invalid email format");
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            result.AddError("Password is required");
        }

        if (string.IsNullOrWhiteSpace(request.Role))
        {
            result.AddError("Role is required");
        }
        else
        {
            var validRoles = new[] { "STUDENT", "ADVISOR", "ACADEMICIAN","ADMIN" };
            if (!validRoles.Contains(request.Role.ToUpper()))
            {
                result.AddError("Invalid role. Must be STUDENT, ADVISOR, or ACADEMICIAN");
            }
        }

        return result;
    }
}