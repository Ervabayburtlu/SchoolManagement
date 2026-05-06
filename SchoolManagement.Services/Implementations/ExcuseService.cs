using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class ExcuseService : IExcuseService
{
    private readonly IExcuseRepository _excuseRepository;
    private readonly IConsistencyService _consistencyService;

    public ExcuseService(IExcuseRepository excuseRepository, IConsistencyService consistencyService)
    {
        _excuseRepository = excuseRepository;
        _consistencyService = consistencyService;
    }

    public async Task<ExcuseDetailResponseDto?> GetByIdAsync(string excuseId)
    {
        var excuse = await _excuseRepository.GetByIdWithDetailsAsync(excuseId);
        if (excuse == null)
            return null;

        return MapToResponseDto(excuse);
    }

    public async Task<IEnumerable<ExcuseDetailResponseDto>> GetByStudentAsync(string studentNo)
    {
        var excuses = await _excuseRepository.GetExcusesByStudentAsync(studentNo);
        return excuses.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<ExcuseDetailResponseDto>> GetByAdvisorAsync(string advisorId)
    {
        var excuses = await _excuseRepository.GetExcusesByAdvisorAsync(advisorId);
        return excuses.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<ExcuseDetailResponseDto>> GetPendingExcusesAsync()
    {
        var excuses = await _excuseRepository.GetPendingExcusesAsync();
        return excuses.Select(MapToResponseDto);
    }

    public async Task<ExcuseDetailResponseDto> CreateAsync(ExcuseCreateDto request)
    {
        var excuse = new Excuse
        {
            ExcuseId = Guid.NewGuid().ToString(),
            StudentNo = request.StudentNo,
            ExamId = request.ExamId,
            ExcuseDescription = request.ExcuseDescription,
            DocumentPath = request.DocumentPath,
            RequestDate = DateTime.UtcNow,
            Status = "PENDING"
        };

        var created = await _excuseRepository.AddAsync(excuse);

        // GEÇİCİ: consistency'yi atla, hata buradan mı geliyor test et
        try 
        {
            await _consistencyService.OnExcuseSubmittedAsync(request.StudentNo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Consistency hatası (kritik değil): {ex.Message}");
            // Devam et, bu yüzden başvuru başarısız olmasın
        }

        var detailed = await _excuseRepository.GetByIdWithDetailsAsync(created.ExcuseId);
    
        // GEÇİCİ: detailed null mu kontrol et
        if (detailed == null)
        {
            Console.WriteLine($"HATA: GetByIdWithDetailsAsync null döndü. ExcuseId: {created.ExcuseId}");
            return MapToResponseDto(created); // ham entity ile dön
        }
    
        return MapToResponseDto(detailed);
    }
    public async Task<ExcuseDetailResponseDto> RespondToExcuseAsync(string excuseId, ExcuseResponseDto request)
    {
        var excuse = await _excuseRepository.GetByIdAsync(excuseId);
        if (excuse == null)
            throw new KeyNotFoundException($"Excuse with ID {excuseId} not found");

        excuse.Status = request.Status;
        excuse.ResponseDate = DateTime.UtcNow;

        await _excuseRepository.UpdateAsync(excuse);

        if (request.Status == "APPROVED")
            await _consistencyService.OnExcuseApprovedAsync(excuse.StudentNo);
        else if (request.Status == "REJECTED")
            await _consistencyService.OnExcuseRejectedAsync(excuse.StudentNo);

        var detailed = await _excuseRepository.GetByIdWithDetailsAsync(excuseId);
        return MapToResponseDto(detailed!);
    }

    public async Task<bool> DeleteAsync(string excuseId)
    {
        var excuse = await _excuseRepository.GetByIdAsync(excuseId);
        if (excuse == null)
            return false;

        await _excuseRepository.DeleteAsync(excuse);
        return true;
    }

    private static ExcuseDetailResponseDto MapToResponseDto(Excuse excuse)
    {
        return new ExcuseDetailResponseDto
        {
            ExcuseId = excuse.ExcuseId,
            StudentNo = excuse.StudentNo,
            StudentName = excuse.Student?.NameSurname ?? string.Empty,
            ExamId = excuse.ExamId,
            // Entity üzerinden ilişkili tablo verilerini çekiyoruz:
            SubjectName = excuse.Exam?.Subject?.SubjectName ?? "Bilinmiyor",
            ExamType = excuse.Exam?.ExamType ?? "Bilinmiyor",
            RequestDate = excuse.RequestDate,
            ExcuseDescription = excuse.ExcuseDescription,
            ResponseDate = excuse.ResponseDate,
            DocumentPath = excuse.DocumentPath,
            Status = excuse.Status
        };
    }

}