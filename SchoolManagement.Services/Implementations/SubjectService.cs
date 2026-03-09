using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class SubjectService : ISubjectService
{
    private readonly ISubjectRepository _subjectRepository;

    public SubjectService(ISubjectRepository subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<SubjectResponseDto?> GetByIdAsync(string subjectId)
    {
        var subject = await _subjectRepository.GetByIdWithDetailsAsync(subjectId);
        if (subject == null)
            return null;

        return MapToResponseDto(subject);
    }

    public async Task<IEnumerable<SubjectResponseDto>> GetAllAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync();
        return subjects.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<SubjectResponseDto>> GetByAcademicianAsync(string academicianId)
    {
        var subjects = await _subjectRepository.GetSubjectsByAcademicianAsync(academicianId);
        return subjects.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<SubjectResponseDto>> GetByStudentAsync(string studentNo)
    {
        var subjects = await _subjectRepository.GetSubjectsByStudentAsync(studentNo);
        return subjects.Select(MapToResponseDto);
    }

    public async Task<SubjectResponseDto> CreateAsync(SubjectCreateDto request)
    {
        var subject = new Subject
        {
            SubjectId = request.SubjectId,
            SubjectName = request.SubjectName,
            AcademicianId = request.AcademicianId,
            DayIndex = request.DayIndex,
            StartTime = request.StartTime,
            EndTime = request.EndTime
        };

        var created = await _subjectRepository.AddAsync(subject);
        return MapToResponseDto(created);
    }

    public async Task<bool> DeleteAsync(string subjectId)
    {
        var subject = await _subjectRepository.GetByIdAsync(subjectId);
        if (subject == null)
            return false;

        await _subjectRepository.DeleteAsync(subject);
        return true;
    }

    public async Task<bool> ExistsAsync(string subjectId)
    {
        return await _subjectRepository.ExistsAsync(s => s.SubjectId == subjectId);
    }

    private static SubjectResponseDto MapToResponseDto(Subject subject)
    {
        return new SubjectResponseDto
        {
            SubjectId = subject.SubjectId,
            SubjectName = subject.SubjectName,
            AcademicianId = subject.AcademicianId,
            AcademicianName = subject.Academician != null 
                ? $"{subject.Academician.FirstName} {subject.Academician.LastName}" 
                : null,
            DayIndex = subject.DayIndex,
            StartTime = subject.StartTime,
            EndTime = subject.EndTime     
        };
    }
}
