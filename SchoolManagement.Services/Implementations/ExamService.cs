using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;
    private readonly IStudentExamRepository _studentExamRepository;

    public ExamService(IExamRepository examRepository, IStudentExamRepository studentExamRepository)
    {
        _examRepository = examRepository;
        _studentExamRepository = studentExamRepository;
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
        // 1. Öðrencinin sýnavlarýný DB'den çek (Muhtemelen Repository'de Include(e => e.StudentExams) yapýlýyordur)
        var exams = await _examRepository.GetExamsByStudentAsync(studentNo);

        // 2. Her bir sýnavý DTO'ya dönüþtürürken öðrencinin durumunu da içine ekle
        return exams.Select(exam =>
        {
            var dto = MapToResponseDto(exam);

            // Öðrencinin bu sýnava ait StudentExam (Katýlým) kaydýný bul
            var studentExam = exam.StudentExams.FirstOrDefault(se => se.StudentNo == studentNo);

            // Eðer kayýt varsa veritabanýndaki durumu DTO'ya yaz, yoksa PENDING (Beklemede) yap
            dto.Status = studentExam?.ParticipationStatus ?? "PENDING";

            return dto;
        });
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

    public async Task<bool> UpdateStatusAsync(string examId, string studentNo, string status)
    {
        // Repository'de halihazýrda tanýmlý olan özel metodu kullanýyoruz
        var studentExam = await _studentExamRepository.GetByStudentAndExamAsync(studentNo, examId);

        if (studentExam == null)
            return false;

        // Öðrencinin o sýnava ait katýlým durumunu güncelliyoruz
        studentExam.ParticipationStatus = status;

        // Generic repository'den gelen güncelleme metodunu çaðýrýyoruz
        await _studentExamRepository.UpdateAsync(studentExam);
        return true;
    }
}

