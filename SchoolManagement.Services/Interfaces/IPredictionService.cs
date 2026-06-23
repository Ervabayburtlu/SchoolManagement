using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;

namespace SchoolManagement.Services.Interfaces;

public interface IPredictionService
{
    Task<ExamPredictionResponseDto> PredictExamAttendanceAsync(ExamPredictionRequestDto request);
}