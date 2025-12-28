using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class AcademicianService : IAcademicianService
{
    private readonly IAcademicianRepository _academicianRepository;

    public AcademicianService(IAcademicianRepository academicianRepository)
    {
        _academicianRepository = academicianRepository;
    }

    public async Task<AcademicianResponseDto?> GetByIdAsync(string academicianId)
    {
        var academician = await _academicianRepository.GetByIdAsync(academicianId);
        if (academician == null)
            return null;

        return MapToResponseDto(academician);
    }

    public async Task<IEnumerable<AcademicianResponseDto>> GetAllAsync()
    {
        var academicians = await _academicianRepository.GetAllAsync();
        return academicians.Select(MapToResponseDto);
    }

    public async Task<AcademicianResponseDto> CreateAsync(AcademicianCreateDto request)
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var academician = new Academician
        {
            AcademicianId = request.AcademicianId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            AcademicianEmail = request.AcademicianEmail,
            AcademicianPhone = request.AcademicianPhone,
            Password = hashedPassword
        };

        var created = await _academicianRepository.AddAsync(academician);
        return MapToResponseDto(created);
    }

    public async Task<bool> DeleteAsync(string academicianId)
    {
        var academician = await _academicianRepository.GetByIdAsync(academicianId);
        if (academician == null)
            return false;

        await _academicianRepository.DeleteAsync(academician);
        return true;
    }

    public async Task<bool> ExistsAsync(string academicianId)
    {
        return await _academicianRepository.ExistsAsync(a => a.AcademicianId == academicianId);
    }

    private static AcademicianResponseDto MapToResponseDto(Academician academician)
    {
        return new AcademicianResponseDto
        {
            AcademicianId = academician.AcademicianId,
            FirstName = academician.FirstName,
            LastName = academician.LastName,
            AcademicianEmail = academician.AcademicianEmail,
            AcademicianPhone = academician.AcademicianPhone
        };
    }
}