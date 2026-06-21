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
    private readonly ReCaptchaService _reCaptchaService;

    public AuthController(IAuthService authService, ReCaptchaService reCaptchaService)
    {
        _authService = authService;
        _reCaptchaService = reCaptchaService;
    }

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
}