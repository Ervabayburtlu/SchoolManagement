using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Enums;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Infrastructure.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class ExamService : IExamService
{
    private readonly IExamRepository _examRepository;
    private readonly IStudentExamRepository _studentExamRepository;
    private readonly IConsistencyService _consistencyService;

    public ExamService(IExamRepository examRepository, IStudentExamRepository studentExamRepository, IConsistencyService consistencyService)
    {
        _examRepository = examRepository;
        _studentExamRepository = studentExamRepository;
        _consistencyService = consistencyService;
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

        return exams.Select(exam =>
        {
            var dto = MapToResponseDto(exam);

            var studentExam = exam.StudentExams.FirstOrDefault(se => se.StudentNo == studentNo);

            // 1. Kat�l�m Durumunu (Enum) DTO'ya aktar�yoruz
            dto.ParticipationStatus = studentExam != null
                ? studentExam.ParticipationStatus.ToString()
                : ParticipationStatus.Bekliyor.ToString();

            // 2. Bildirim Durumunu DTO'ya aktar�yoruz
            dto.ParticipationNotification = studentExam?.ParticipationNotification ?? string.Empty;

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

    public async Task<bool> UpdateStatusAsync(string examId, string studentNo, string status, string? notification = null)
    {
        var studentExam = await _studentExamRepository.GetByStudentAndExamAsync(studentNo, examId);
        if (studentExam == null) return false;

        string studentPlan = status;

        if (!string.IsNullOrEmpty(notification) && notification != status)
            studentPlan = $"{status} - {notification}";

        if (studentPlan.Length > 50)
            studentPlan = studentPlan.Substring(0, 50);

        studentExam.ParticipationNotification = studentPlan;
        await _studentExamRepository.UpdateAsync(studentExam);

        return true;
    }

    public async Task<bool> UpdateParticipationAsync(string examId, string studentNo, ParticipationStatus newStatus)
    {
        var studentExam = await _studentExamRepository.GetByStudentAndExamAsync(studentNo, examId);
        if (studentExam == null) return false;

        var prevStatus = studentExam.ParticipationStatus;
        var notification = studentExam.ParticipationNotification ?? string.Empty;

        studentExam.ParticipationStatus = newStatus;
        await _studentExamRepository.UpdateAsync(studentExam);

        // Yoklama ilk kez alınıyorsa değerlendir
        if (prevStatus != ParticipationStatus.Bekliyor) return true;

        if (string.IsNullOrWhiteSpace(notification))
        {
            // Kural 3: Hiç bildirim yok
            await _consistencyService.OnAbsentWithoutNotificationAsync(studentNo);
        }
        else if (notification == "APPROVED" && newStatus == ParticipationStatus.Katılmadı)
        {
            // Kural 1: Katılacağım dedi, katılmadı
            await _consistencyService.OnInconsistentBehaviorAsync(studentNo);
        }
        else if (notification == "REJECTED" && newStatus == ParticipationStatus.Katıldı)
        {
            // Kural 2: Katılmayacağım dedi, katıldı
            await _consistencyService.OnInconsistentBehaviorAsync(studentNo);
        }
        // APPROVED+Katıldı veya REJECTED+Katılmadı → bildirimle uyuşuyor, bar yok

        return true;
    }

    public async Task<IEnumerable<object>> GetStudentsByExamAsync(string examId)
    {
        var studentExams = await _studentExamRepository.GetByExamAsync(examId);
        return studentExams.Select(se => new {
            studentNo = se.StudentNo,
            studentName = se.Student?.NameSurname ?? string.Empty,
            participationStatus = se.ParticipationStatus.ToString(),
            participationNotification = se.ParticipationNotification
        });
    }
}

