using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class AdvisorService : IAdvisorService
{
    private readonly IAdvisorRepository _advisorRepository;

    public AdvisorService(IAdvisorRepository advisorRepository)
    {
        _advisorRepository = advisorRepository;
    }

    public async Task<AdvisorResponseDto?> GetByIdAsync(string advisorId)
    {
        var advisor = await _advisorRepository.GetByIdWithStudentsAsync(advisorId);
        if (advisor == null)
            return null;

        return MapToResponseDto(advisor);
    }

    public async Task<IEnumerable<AdvisorResponseDto>> GetAllAsync()
    {
        var advisors = await _advisorRepository.GetAllAsync();
        return advisors.Select(MapToResponseDto);
    }

    public async Task<AdvisorResponseDto> CreateAsync(AdvisorCreateDto request)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var advisor = new Advisor
        {
            AdvisorId = request.AdvisorId,
            NameSurname = request.NameSurname,
            AdvisorMail = request.AdvisorMail,
            Password = hashedPassword
        };

        var created = await _advisorRepository.AddAsync(advisor);
        return MapToResponseDto(created);
    }

    public async Task<bool> DeleteAsync(string advisorId)
    {
        var advisor = await _advisorRepository.GetByIdAsync(advisorId);
        if (advisor == null)
            return false;

        await _advisorRepository.DeleteAsync(advisor);
        return true;
    }

    public async Task<bool> ExistsAsync(string advisorId)
    {
        return await _advisorRepository.ExistsAsync(a => a.AdvisorId == advisorId);
    }

    private static AdvisorResponseDto MapToResponseDto(Advisor advisor)
    {
        return new AdvisorResponseDto
        {
            AdvisorId = advisor.AdvisorId,
            NameSurname = advisor.NameSurname,
            AdvisorMail = advisor.AdvisorMail,
            StudentCount = advisor.Students?.Count ?? 0
        };
    }
}

