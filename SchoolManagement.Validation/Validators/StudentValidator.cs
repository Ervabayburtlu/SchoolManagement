using SchoolManagement.Validation.Models;
using SchoolManagement.Validation.Validators.Common;
using SchoolManagement.Core.DTOs.Request;

namespace SchoolManagement.Validation.Validators;

public static class StudentValidator
{
    public static ValidationResult ValidateCreate(StudentCreateDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.StudentNo))
        {
            result.AddError("Student number is required");
        }

        if (string.IsNullOrWhiteSpace(request.NameSurname))
        {
            result.AddError("Name surname is required");
        }
        else if (request.NameSurname.Length < 3)
        {
            result.AddError("Name surname must be at least 3 characters");
        }

        if (string.IsNullOrWhiteSpace(request.Grade))
        {
            result.AddError("Grade is required");
        }

        if (request.GPA < 0 || request.GPA > 4)
        {
            result.AddError("GPA must be between 0 and 4");
        }

        if (string.IsNullOrWhiteSpace(request.StudentMail))
        {
            result.AddError("Email is required");
        }
        else if (!EmailValidator.IsValidEmail(request.StudentMail))
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

    public static ValidationResult ValidateUpdate(StudentUpdateDto request)
    {
        var result = new ValidationResult { IsValid = true };

        if (string.IsNullOrWhiteSpace(request.NameSurname))
        {
            result.AddError("Name surname is required");
        }
        else if (request.NameSurname.Length < 3)
        {
            result.AddError("Name surname must be at least 3 characters");
        }

        if (string.IsNullOrWhiteSpace(request.Grade))
        {
            result.AddError("Grade is required");
        }

        if (request.GPA < 0 || request.GPA > 4)
        {
            result.AddError("GPA must be between 0 and 4");
        }

        if (string.IsNullOrWhiteSpace(request.StudentMail))
        {
            result.AddError("Email is required");
        }
        else if (!EmailValidator.IsValidEmail(request.StudentMail))
        {
            result.AddError("Invalid email format");
        }

        return result;
    }
}
