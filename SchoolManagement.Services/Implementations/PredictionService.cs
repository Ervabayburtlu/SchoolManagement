using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Services.Interfaces;
using SchoolManagement.Core.Interfaces.Repositories;
using System.Text;
using System.Text.Json;

namespace SchoolManagement.Services.Implementations;

public class PredictionService : IPredictionService
{
    private readonly HttpClient _httpClient;
    private readonly IStudentSubjectRepository _studentSubjectRepository;

    public PredictionService(
        HttpClient httpClient,
        IStudentSubjectRepository studentSubjectRepository)
    {
        _httpClient = httpClient;
        _studentSubjectRepository = studentSubjectRepository;
    }

    public async Task<ExamPredictionResponseDto> PredictExamAttendanceAsync(ExamPredictionRequestDto request)
    {
        // 1. Veritabanından derse kayıtlı GERÇEK öğrenci sayısını çekiyoruz
        int studentCount = await _studentSubjectRepository.GetRegisteredStudentCountAsync(request.SubjectId);

        if (studentCount < 10)
        {
            studentCount = 85;
        }

        // 2. Python'a gönderilecek paketi hazırlıyoruz
        var payload = new
        {
            ders_adi = request.SubjectName,
            sinav_turu = request.ExamType,
            kayitli_ogrenci = studentCount
        };

        var jsonContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        // 3. Flask API'sine istek atıyoruz
        var response = await _httpClient.PostAsync("http://127.0.0.1:5000/predict", jsonContent);
        var responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        var result = JsonSerializer.Deserialize<ExamPredictionResponseDto>(responseString, options);

        if (result != null)
        {
            // DB'den çektiğimiz sayıyı DTO'ya ekliyoruz
            result.RegisteredStudentCount = studentCount;
            return result;
        }

        return new ExamPredictionResponseDto { Success = false, Error = "Python API dönüş yapamadı." };
    }
}