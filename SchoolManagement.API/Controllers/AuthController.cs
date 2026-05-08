using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Services.Interfaces;
using SchoolManagement.Validation.Validators;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            // Validation
            var validationResult = AuthValidator.ValidateLogin(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Validation failed",
                    validationResult.Errors));
            }

            var result = await _authService.LoginAsync(request);

            if (result == null)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("Invalid credentials"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(result, "Login successful"));
        }
        catch (AccountLockedException ex) // AccountLockedException class'ýnýn tanýmlý olduðunu varsayýyorum
        {
            return StatusCode(403, new
            {
                success = false,
                message = "ACCOUNT_LOCKED",
                data = new
                {
                    message = "Hesabýnýz kilitlenmiþtir. Danýþmanýnýzla iletiþime geçin.",
                    advisorName = ex.AdvisorName
                }
            });
        }
    }

    // OBS GÝRÝÞ ENDPOINT'Ý
    [HttpPost("obs-login")]
    public async Task<IActionResult> ObsLogin([FromBody] LoginRequestDto request)
    {
        try
        {
            // OBS giriþi için de temel validasyonlarý (boþ deðer kontrolü vb.) yapýyoruz.
            // Eðer OBS için spesifik kurallar varsa AuthValidator içine ValidateObsLogin yazabilirsin.
            var validationResult = AuthValidator.ValidateLogin(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Validation failed",
                    validationResult.Errors));
            }

            // Servis katmanýnda OBS'e özel mantýðý iþleyecek metodu çaðýrýyoruz
            var result = await _authService.ObsLoginAsync(request);

            if (result == null)
            {
                return Unauthorized(ApiResponse<object>.ErrorResponse("OBS giriþ bilgileri hatalý"));
            }

            return Ok(ApiResponse<object>.SuccessResponse(result, "OBS login successful"));
        }
        catch (Exception ex)
        {
            // OBS API'si ile haberleþirken yaþanabilecek sorunlar için genel hata yakalama
            return StatusCode(500, ApiResponse<object>.ErrorResponse("OBS giriþ iþlemi sýrasýnda sunucu hatasý oluþtu."));
        }
    }
}