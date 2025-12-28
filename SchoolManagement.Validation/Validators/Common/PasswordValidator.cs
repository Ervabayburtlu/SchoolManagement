using System.Text.RegularExpressions;

namespace SchoolManagement.Validation.Validators.Common;

public static class PasswordValidator
{
    public static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // En az 6 karakter
        if (password.Length < 6)
            return false;

        return true;
    }

    public static bool IsStrongPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;

        // En az 8 karakter, bir büyük harf, bir küçük harf, bir rakam
        if (password.Length < 8)
            return false;

        if (!Regex.IsMatch(password, @"[A-Z]"))
            return false;

        if (!Regex.IsMatch(password, @"[a-z]"))
            return false;

        if (!Regex.IsMatch(password, @"[0-9]"))
            return false;

        return true;
    }
}