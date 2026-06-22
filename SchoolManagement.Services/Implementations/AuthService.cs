using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using SchoolManagement.Core.Common;

namespace SchoolManagement.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IStudentRepository _studentRepository;
    private readonly IAdvisorRepository _advisorRepository;
    private readonly IAcademicianRepository _academicianRepository;
    private readonly IConfiguration _configuration;

    private const int AccessTokenMinutes = 30;
    private const int RefreshTokenDays = 7;

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
                    // Admin için refresh token desteklenmiyor (DB kaydı yok)
                    RefreshToken = string.Empty,
                    UserId = "ADMIN001",
                    Email = adminEmail,
                    Role = "ADMIN",
                    Name = adminName,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes)
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

                var studentToken = GenerateJwtToken(student.StudentNo, student.StudentMail, "STUDENT", student.NameSurname);
                var studentRefreshToken = GenerateRefreshToken();

                student.RefreshToken = studentRefreshToken;
                student.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenDays);
                await _studentRepository.UpdateAsync(student);

                return new LoginResponseDto
                {
                    Token = studentToken,
                    RefreshToken = studentRefreshToken,
                    UserId = student.StudentNo,
                    Email = student.StudentMail,
                    Role = "STUDENT",
                    Name = student.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes)
                };

            case "ADVISOR":
                var advisor = await _advisorRepository.GetByEmailAsync(request.Email);
                if (advisor == null)
                    return null;

                if (!BCrypt.Net.BCrypt.Verify(request.Password, advisor.Password))
                    return null;

                var advisorToken = GenerateJwtToken(advisor.AdvisorId, advisor.AdvisorMail, "ADVISOR", advisor.NameSurname);
                var advisorRefreshToken = GenerateRefreshToken();

                advisor.RefreshToken = advisorRefreshToken;
                advisor.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenDays);
                await _advisorRepository.UpdateAsync(advisor);

                return new LoginResponseDto
                {
                    Token = advisorToken,
                    RefreshToken = advisorRefreshToken,
                    UserId = advisor.AdvisorId,
                    Email = advisor.AdvisorMail,
                    Role = "ADVISOR",
                    Name = advisor.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes)
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

                var academicianRefreshToken = GenerateRefreshToken();

                academician.RefreshToken = academicianRefreshToken;
                academician.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenDays);
                await _academicianRepository.UpdateAsync(academician);

                return new LoginResponseDto
                {
                    Token = academicianToken,
                    RefreshToken = academicianRefreshToken,
                    UserId = academician.AcademicianId,
                    Email = academician.AcademicianEmail,
                    Role = "ACADEMICIAN",
                    Name = $"{academician.FirstName} {academician.LastName}",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes)
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
        var studentRefreshToken = GenerateRefreshToken();

        student.RefreshToken = studentRefreshToken;
        student.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenDays);
        await _studentRepository.UpdateAsync(student);

        return new LoginResponseDto
        {
            Token = studentToken,
            RefreshToken = studentRefreshToken,
            UserId = student.StudentNo,
            Email = student.StudentMail,
            Role = "STUDENT",
            Name = student.NameSurname,
            ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes)
        };
    }

    // REFRESH TOKEN METODU
    public async Task<LoginResponseDto?> RefreshTokenAsync(RefreshRequestDto request)
    {
        var role = request.Role.ToUpper();

        switch (role)
        {
            case "STUDENT":
                var student = await _studentRepository.GetByIdAsync(request.UserId);
                if (student == null || !IsRefreshTokenValid(student.RefreshToken, student.RefreshTokenExpiry, request.RefreshToken))
                    return null;

                var newStudentToken = GenerateJwtToken(student.StudentNo, student.StudentMail, "STUDENT", student.NameSurname);
                var newStudentRefreshToken = GenerateRefreshToken();

                student.RefreshToken = newStudentRefreshToken;
                student.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenDays);
                await _studentRepository.UpdateAsync(student);

                return new LoginResponseDto
                {
                    Token = newStudentToken,
                    RefreshToken = newStudentRefreshToken,
                    UserId = student.StudentNo,
                    Email = student.StudentMail,
                    Role = "STUDENT",
                    Name = student.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes)
                };

            case "ADVISOR":
                var advisor = await _advisorRepository.GetByIdAsync(request.UserId);
                if (advisor == null || !IsRefreshTokenValid(advisor.RefreshToken, advisor.RefreshTokenExpiry, request.RefreshToken))
                    return null;

                var newAdvisorToken = GenerateJwtToken(advisor.AdvisorId, advisor.AdvisorMail, "ADVISOR", advisor.NameSurname);
                var newAdvisorRefreshToken = GenerateRefreshToken();

                advisor.RefreshToken = newAdvisorRefreshToken;
                advisor.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenDays);
                await _advisorRepository.UpdateAsync(advisor);

                return new LoginResponseDto
                {
                    Token = newAdvisorToken,
                    RefreshToken = newAdvisorRefreshToken,
                    UserId = advisor.AdvisorId,
                    Email = advisor.AdvisorMail,
                    Role = "ADVISOR",
                    Name = advisor.NameSurname,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes)
                };

            case "ACADEMICIAN":
                var academician = await _academicianRepository.GetByIdAsync(request.UserId);
                if (academician == null || !IsRefreshTokenValid(academician.RefreshToken, academician.RefreshTokenExpiry, request.RefreshToken))
                    return null;

                var newAcademicianToken = GenerateJwtToken(
                    academician.AcademicianId,
                    academician.AcademicianEmail,
                    "ACADEMICIAN",
                    $"{academician.FirstName} {academician.LastName}");
                var newAcademicianRefreshToken = GenerateRefreshToken();

                academician.RefreshToken = newAcademicianRefreshToken;
                academician.RefreshTokenExpiry = DateTime.UtcNow.AddDays(RefreshTokenDays);
                await _academicianRepository.UpdateAsync(academician);

                return new LoginResponseDto
                {
                    Token = newAcademicianToken,
                    RefreshToken = newAcademicianRefreshToken,
                    UserId = academician.AcademicianId,
                    Email = academician.AcademicianEmail,
                    Role = "ACADEMICIAN",
                    Name = $"{academician.FirstName} {academician.LastName}",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(AccessTokenMinutes)
                };

            default:
                // ADMIN için refresh desteklenmiyor
                return null;
        }
    }

    // LOGOUT METODU - refresh token'ı geçersizleştirir
    public async Task LogoutAsync(string userId, string role)
    {
        switch (role.ToUpper())
        {
            case "STUDENT":
                var student = await _studentRepository.GetByIdAsync(userId);
                if (student != null)
                {
                    student.RefreshToken = null;
                    student.RefreshTokenExpiry = null;
                    await _studentRepository.UpdateAsync(student);
                }
                break;

            case "ADVISOR":
                var advisor = await _advisorRepository.GetByIdAsync(userId);
                if (advisor != null)
                {
                    advisor.RefreshToken = null;
                    advisor.RefreshTokenExpiry = null;
                    await _advisorRepository.UpdateAsync(advisor);
                }
                break;

            case "ACADEMICIAN":
                var academician = await _academicianRepository.GetByIdAsync(userId);
                if (academician != null)
                {
                    academician.RefreshToken = null;
                    academician.RefreshTokenExpiry = null;
                    await _academicianRepository.UpdateAsync(academician);
                }
                break;

            default:
                // ADMIN için yapılacak bir şey yok
                break;
        }
    }

    private static bool IsRefreshTokenValid(string? storedToken, DateTime? storedExpiry, string providedToken)
    {
        if (string.IsNullOrEmpty(storedToken) || storedExpiry == null)
            return false;

        if (storedExpiry < DateTime.UtcNow)
            return false;

        return storedToken == providedToken;
    }

    private static string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
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
            expires: DateTime.UtcNow.AddMinutes(AccessTokenMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}