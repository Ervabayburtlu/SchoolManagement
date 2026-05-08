using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using SchoolManagement.Core.Common;

namespace SchoolManagement.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IAdvisorRepository _advisorRepository;
    private readonly IAcademicianRepository _academicianRepository;
    private readonly IConfiguration _configuration;

    public AuthService(
        IStudentRepository studentRepository,
        IAdvisorRepository advisorRepository,
        IAcademicianRepository academicianRepository,
        IConfiguration configuration)
    {
        _studentRepository = studentRepository;
        _advisorRepository = advisorRepository;
        _academicianRepository = academicianRepository;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var role = request.Role.ToUpper();

        // 1. Admin Kontrolü (Kod tarafında/Appsettings'ten)
        if (role == "ADMIN")
        {
            var adminSettings = _configuration.GetSection("AdminSettings");
            var adminEmail = adminSettings["Email"];
            var adminPassword = adminSettings["Password"];
            var adminName = adminSettings["Name"] ?? "Admin";

            // Admin e-posta ve şifre uyuşuyor mu?
            if (request.Email == adminEmail && request.Password == adminPassword)
            {
                var adminToken = GenerateJwtToken("ADMIN001", adminEmail, "ADMIN", adminName);
                return new LoginResponseDto
                {
                    Token = adminToken,
                    UserId = "ADMIN001",
                    Email = adminEmail,
                    Role = "ADMIN",
                    Name = adminName,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };
            }
            return null; // Bilgiler hatalıysa
        }

        switch (role)
        {
            case "STUDENT":
                var student = await _studentRepository.GetByEmailAsync(request.Email);
                if (student == null)
                    return null;

                if (!BCrypt.Net.BCrypt.Verify(request.Password, student.Password))
                    return null;

                // ✅ Önce student'ı çek, sonra kilit kontrolü yap
                if (student.IsLocked)
                    throw new AccountLockedException(
                        student.Advisor?.NameSurname ?? null
                    );

                var studentToken = GenerateJwtToken(student.StudentNo, student.StudentMail, "STUDENT", student.NameSurname);
                return new LoginResponseDto
                {
                    Token = studentToken,
                    UserId = student.StudentNo,
                    Email = student.StudentMail,
                    Role = "STUDENT",
                    Name = student.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

            case "ADVISOR":
                var advisor = await _advisorRepository.GetByEmailAsync(request.Email);
                if (advisor == null)
                    return null;

                if (!BCrypt.Net.BCrypt.Verify(request.Password, advisor.Password))
                    return null;

                var advisorToken = GenerateJwtToken(advisor.AdvisorId, advisor.AdvisorMail, "ADVISOR", advisor.NameSurname);
                return new LoginResponseDto
                {
                    Token = advisorToken,
                    UserId = advisor.AdvisorId,
                    Email = advisor.AdvisorMail,
                    Role = "ADVISOR",
                    Name = advisor.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

            case "ACADEMICIAN":
                var academician = await _academicianRepository.GetByEmailAsync(request.Email);
                if (academician == null)
                    return null;

                if (!BCrypt.Net.BCrypt.Verify(request.Password, academician.Password))
                    return null;

                var academicianToken = GenerateJwtToken(
                    academician.AcademicianId,
                    academician.AcademicianEmail,
                    "ACADEMICIAN",
                    $"{academician.FirstName} {academician.LastName}");

                return new LoginResponseDto
                {
                    Token = academicianToken,
                    UserId = academician.AcademicianId,
                    Email = academician.AcademicianEmail,
                    Role = "ACADEMICIAN",
                    Name = $"{academician.FirstName} {academician.LastName}",
                    ExpiresAt = DateTime.UtcNow.AddHours(24)
                };

            default:
                return null;
        }
    }

    // OBS GİRİŞ METODU
    public async Task<LoginResponseDto?> ObsLoginAsync(LoginRequestDto request)
    {
        // OBS girişi istendiğinde direkt Öğrenci (Student) tablosundan kontrol ediyoruz.
        var student = await _studentRepository.GetByEmailAsync(request.Email);

        if (student == null)
            return null;

        // Şifre kontrolü
        if (!BCrypt.Net.BCrypt.Verify(request.Password, student.Password))
            return null;

        // OBS girişinde de hesabın kilitli olup olmadığına bakmak istersen bu bloğu tutabilirsin.
        // İstemiyorsan silebilirsin.
        if (student.IsLocked)
            throw new AccountLockedException(
                student.Advisor?.NameSurname ?? null
            );

        // JWT Token Üretimi (OBS kullanıcısı olduğunu ayırt etmek istersen Role kısmını "OBS_STUDENT" yapabilirsin, 
        // ancak sistemdeki diğer authorization yapılarını bozmamak için şimdilik "STUDENT" bıraktım)
        var studentToken = GenerateJwtToken(student.StudentNo, student.StudentMail, "STUDENT", student.NameSurname);

        return new LoginResponseDto
        {
            Token = studentToken,
            UserId = student.StudentNo,
            Email = student.StudentMail,
            Role = "STUDENT",
            Name = student.NameSurname,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    public string GenerateJwtToken(string userId, string email, string role, string name)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
        var issuer = jwtSettings["Issuer"] ?? "SchoolManagement";
        var audience = jwtSettings["Audience"] ?? "SchoolManagementUsers";

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(ClaimTypes.Name, name),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(24),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}