using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class ExcuseService : IExcuseService
{
    private readonly IExcuseRepository _excuseRepository;

    public ExcuseService(IExcuseRepository excuseRepository)
    {
        _excuseRepository = excuseRepository;
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
        
        // Detaylı bilgi için tekrar çek
        var detailed = await _excuseRepository.GetByIdWithDetailsAsync(created.ExcuseId);
        return MapToResponseDto(detailed!);
    }

    public async Task<ExcuseDetailResponseDto> RespondToExcuseAsync(string excuseId, ExcuseResponseDto request)
    {
        var excuse = await _excuseRepository.GetByIdAsync(excuseId);
        if (excuse == null)
            throw new KeyNotFoundException($"Excuse with ID {excuseId} not found");

        excuse.Status = request.Status;
        excuse.ResponseDate = DateTime.UtcNow;

        await _excuseRepository.UpdateAsync(excuse);

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
            ExcuseDescription = excuse.ExcuseDescription,
            RequestDate = excuse.RequestDate,
            ResponseDate = excuse.ResponseDate,
            DocumentPath = excuse.DocumentPath,
            Status = excuse.Status
        };
    }
}