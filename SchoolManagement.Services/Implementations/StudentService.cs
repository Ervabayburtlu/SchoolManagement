using SchoolManagement.Core.DTOs.Request;
using SchoolManagement.Core.DTOs.Response;
using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;

namespace SchoolManagement.Services.Implementations;

public class StudentService : IStudentService
{
    private readonly IStudentRepository _studentRepository;

    public StudentService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<StudentResponseDto?> GetByIdAsync(string studentNo)
    {
        var student = await _studentRepository.GetByStudentNoWithDetailsAsync(studentNo);
        if (student == null)
            return null;

        return MapToResponseDto(student);
    }

    public async Task<IEnumerable<StudentResponseDto>> GetAllAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return students.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<StudentResponseDto>> GetByAdvisorAsync(string advisorId)
    {
        var students = await _studentRepository.GetStudentsByAdvisorAsync(advisorId);
        return students.Select(MapToResponseDto);
    }

    public async Task<IEnumerable<StudentResponseDto>> GetByGradeAsync(string grade)
    {
        var students = await _studentRepository.GetStudentsByGradeAsync(grade);
        return students.Select(MapToResponseDto);
    }

    public async Task<StudentResponseDto> CreateAsync(StudentCreateDto request)
    {
        // Hash password
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var student = new Student
        {
            StudentNo = request.StudentNo,
            NameSurname = request.NameSurname,
            Grade = request.Grade,
            GPA = request.GPA,
            StudentMail = request.StudentMail,
            Password = hashedPassword,
            AdvisorId = request.AdvisorId
        };

        var created = await _studentRepository.AddAsync(student);
        return MapToResponseDto(created);
    }

    public async Task<StudentResponseDto> UpdateAsync(string studentNo, StudentUpdateDto request)
    {
        var student = await _studentRepository.GetByIdAsync(studentNo);
        if (student == null)
            throw new KeyNotFoundException($"Student with number {studentNo} not found");

        student.NameSurname = request.NameSurname;
        student.Grade = request.Grade;
        student.GPA = request.GPA;
        student.StudentMail = request.StudentMail;
        student.AdvisorId = request.AdvisorId;

        await _studentRepository.UpdateAsync(student);
        return MapToResponseDto(student);
    }

    public async Task<bool> DeleteAsync(string studentNo)
    {
        var student = await _studentRepository.GetByIdAsync(studentNo);
        if (student == null)
            return false;

        await _studentRepository.DeleteAsync(student);
        return true;
    }

    public async Task<bool> ExistsAsync(string studentNo)
    {
        return await _studentRepository.ExistsAsync(s => s.StudentNo == studentNo);
    }

    private static StudentResponseDto MapToResponseDto(Student student)
    {
        return new StudentResponseDto
        {
            StudentNo = student.StudentNo,
            NameSurname = student.NameSurname,
            Grade = student.Grade,
            GPA = student.GPA,
            StudentMail = student.StudentMail,
            AdvisorId = student.AdvisorId,
            AdvisorName = student.Advisor?.NameSurname
        };
    }
    public async Task<IEnumerable<StudentResponseDto>> GetInactiveStudentsAsync()
        {
            // Repository üzerinden IsLocked veya benzeri bir durumu true olanları çekiyoruz
            // Not: Repository'de bu metod yoksa önce oraya da eklemen gerekebilir 
            // veya GetAll üzerinden filtreleyebilirsin (performans için repository önerilir)
            
            var inactiveStudents = await _studentRepository.FindAsync(s => s.IsLocked == true);
            
            return inactiveStudents.Select(s => new StudentResponseDto {
                AdvisorId = s.AdvisorId,
                StudentNo = s.StudentNo,
                NameSurname = s.NameSurname,
                IsLocked = s.IsLocked
                // ... diğer maplemeler
            });
        }
}
