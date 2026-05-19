using Microsoft.Extensions.Configuration;
using System.Text.Json;

public class ReCaptchaService
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public ReCaptchaService(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public async Task<bool> VerifyAsync(string token)
    {
        var secretKey = _config["ReCaptcha:SecretKey"];
        var response = await _httpClient.PostAsync(
            $"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={token}",
            null
        );

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<JsonElement>(json);
        return result.GetProperty("success").GetBoolean();
    }
}