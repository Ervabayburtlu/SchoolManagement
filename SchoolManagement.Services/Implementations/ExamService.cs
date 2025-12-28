using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;

    public ExamService(IExamRepository examRepository)
    {
        _examRepository = examRepository;
    }

    public async Task<ExamResponseDto?> GetByIdAsync(string examId)
    {
        var exam = await _examRepository.GetByIdWithDetailsAsync(examId);
        if (exam == null)
            return null;

        return MapToResponseDto(exam);
    }

    public async Task<IEnumerable<ExamResponseDto>> GetAllAsync()
    {
        var exams = await _examRepository.GetAllAsync();
        return exams.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<ExamResponseDto>> GetBySubjectAsync(string subjectId)
    {
        var exams = await _examRepository.GetExamsBySubjectAsync(subjectId);
        return exams.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<ExamResponseDto>> GetByStudentAsync(string studentNo)
    {
        var exams = await _examRepository.GetExamsByStudentAsync(studentNo);
        return exams.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<ExamResponseDto>> GetUpcomingExamsAsync()
    {
        var exams = await _examRepository.GetUpcomingExamsAsync();
        return exams.Select(MapToResponseDto);
    }

    public async Task<ExamResponseDto> CreateAsync(ExamCreateDto request)
    {
        var exam = new Exam
        {
            ExamId = request.ExamId,
            SubjectId = request.SubjectId,
            ExamType = request.ExamType,
            ExamDate = request.ExamDate,
            ExamDescription = request.ExamDescription
        };

        var created = await _examRepository.AddAsync(exam);
        return MapToResponseDto(created);
    }

    public async Task<bool> DeleteAsync(string examId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        if (exam == null)
            return false;

        await _examRepository.DeleteAsync(exam);
        return true;
    }

    public async Task<bool> ExistsAsync(string examId)
    {
        return await _examRepository.ExistsAsync(e => e.ExamId == examId);
    }

    private static ExamResponseDto MapToResponseDto(Exam exam)
    {
        return new ExamResponseDto
        {
            ExamId = exam.ExamId,
            SubjectId = exam.SubjectId,
            SubjectName = exam.Subject?.SubjectName ?? string.Empty,
            ExamType = exam.ExamType,
            ExamDate = exam.ExamDate,
            ExamDescription = exam.ExamDescription
        };
    }
}

