using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Core.Common;
using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Services.Interfaces;
using SchoolManagement.Validation.Validators;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SchoolManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ReCaptchaService _reCaptchaService;

    public AuthController(IAuthService authService, ReCaptchaService reCaptchaService)
    {
        _authService = authService;
        _reCaptchaService = reCaptchaService;
    }

    private string? CurrentUserId => User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                       ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                                       ?? User.FindFirst("sub")?.Value;

    private string? CurrentUserRole => User.FindFirst(ClaimTypes.Role)?.Value;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var captchaValid = await _reCaptchaService.VerifyAsync(request.CaptchaToken);
        if (!captchaValid)
            return BadRequest(ApiResponse<object>.ErrorResponse("Captcha doðrulamasý baþarýsýz."));

        var validationResult = AuthValidator.ValidateLogin(request);
        if (!validationResult.IsValid)
            return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed", validationResult.Errors));

        var result = await _authService.LoginAsync(request);

        if (result == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("E-posta veya parola hatalý."));

        return Ok(ApiResponse<object>.SuccessResponse(result, "Login successful"));
    }

    [HttpPost("obs-login")]
    public async Task<IActionResult> ObsLogin([FromBody] LoginRequestDto request)
    {
        try
        {
            var captchaValid = await _reCaptchaService.VerifyAsync(request.CaptchaToken);
            if (!captchaValid)
                return BadRequest(ApiResponse<object>.ErrorResponse("Captcha doðrulamasý baþarýsýz."));

            var validationResult = AuthValidator.ValidateLogin(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(
                    "Validation failed",
                    validationResult.Errors));
            }

            var result = await _authService.ObsLoginAsync(request);

            if (result == null)
                return Unauthorized(ApiResponse<object>.ErrorResponse("E-posta veya parola hatalý."));

            return Ok(ApiResponse<object>.SuccessResponse(result, "OBS login successful"));
        }
        catch (AccountLockedException)
        {
            return StatusCode(403, ApiResponse<object>.ErrorResponse("ACCOUNT_LOCKED"));
        }
        catch (Exception ex)
        {
            return StatusCode(500, ApiResponse<object>.ErrorResponse("OBS giriþ iþlemi sýrasýnda sunucu hatasý oluþtu."));
        }
    }

    // Access token süresi dolduðunda yeni token almak için
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)
            || string.IsNullOrWhiteSpace(request.Role)
            || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return BadRequest(ApiResponse<object>.ErrorResponse("UserId, Role ve RefreshToken alanlarý zorunludur."));
        }

        var result = await _authService.RefreshTokenAsync(request);

        if (result == null)
            return Unauthorized(ApiResponse<object>.ErrorResponse("Geçersiz veya süresi dolmuþ refresh token."));

        return Ok(ApiResponse<object>.SuccessResponse(result, "Token refreshed successfully"));
    }

    // Refresh token'ý geçersizleþtirir (logout)
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        if (string.IsNullOrEmpty(CurrentUserId) || string.IsNullOrEmpty(CurrentUserRole))
            return Unauthorized(ApiResponse<object>.ErrorResponse("Geçersiz oturum."));

        await _authService.LogoutAsync(CurrentUserId, CurrentUserRole);

        return Ok(ApiResponse<object>.SuccessResponse(null, "Logout successful"));
    }
}