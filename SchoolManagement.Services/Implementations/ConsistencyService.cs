using SchoolManagement.Core.Entities;
using SchoolManagement.Core.Interfaces.Repositories;
using SchoolManagement.Services.Interfaces;

public class ConsistencyService : IConsistencyService
{
    private readonly IStudentRepository _studentRepository;

    public ConsistencyService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    // Mazeret yüklendi → bar ekle
    public async Task OnExcuseSubmittedAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);
        if (student.IsLocked) return;

        student.ActiveBarCount++;

        if (student.ActiveBarCount >= 3)
        {
            student.IsLocked = true;
            student.LockedAt = DateTime.UtcNow;
        }

        await _studentRepository.UpdateAsync(student);
    }

    // Mazeret onaylandı → bar geri al
    public async Task OnExcuseApprovedAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);

        if (student.ActiveBarCount > 0)
            student.ActiveBarCount--;

        await _studentRepository.UpdateAsync(student);
    }

    // Mazeret reddedildi → bar kalır, işlem yok
    public async Task OnExcuseRejectedAsync(string studentNo)
    {
        // bar zaten eklenmişti
    }

    // Danışman kilidi açar → bar sıfırla
    public async Task UnlockAccountAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);

        student.IsLocked = false;
        student.ActiveBarCount = 0;
        student.UnlockedAt = DateTime.UtcNow;

        await _studentRepository.UpdateAsync(student);
    }

    // Danışman bar sayısını manuel düzenler
    public async Task SetBarCountAsync(string studentNo, int count)
    {
        var student = await GetStudentAsync(studentNo);

        student.ActiveBarCount = Math.Clamp(count, 0, 3);

        if (student.ActiveBarCount >= 3 && !student.IsLocked)
        {
            student.IsLocked = true;
            student.LockedAt = DateTime.UtcNow;
        }
        else if (student.ActiveBarCount < 3 && student.IsLocked)
        {
            student.IsLocked = false;
        }

        await _studentRepository.UpdateAsync(student);
    }

    private async Task<Student> GetStudentAsync(string studentNo)
    {
        var student = await _studentRepository.GetByIdAsync(studentNo);
        if (student == null)
            throw new KeyNotFoundException($"Öğrenci bulunamadı: {studentNo}");
        return student;
    }

    public async Task<Student?> GetRecordAsync(string studentNo)
    {
        return await _studentRepository.GetByIdAsync(studentNo);
    }

    // Hiç bildirim yok + katılmadı → bar ekle
    public async Task OnAbsentWithoutNotificationAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);
        if (student.IsLocked) return;

        student.ActiveBarCount++;

        if (student.ActiveBarCount >= 3)
        {
            student.IsLocked = true;
            student.LockedAt = DateTime.UtcNow;
        }

        await _studentRepository.UpdateAsync(student);
    }

    // Bildirime ters davranış → bar ekle
    public async Task OnInconsistentBehaviorAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);
        if (student.IsLocked) return;

        student.ActiveBarCount++;

        if (student.ActiveBarCount >= 3)
        {
            student.IsLocked = true;
            student.LockedAt = DateTime.UtcNow;
        }

        await _studentRepository.UpdateAsync(student);
    }

    public async Task OnPositiveSurpriseAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);

        if (student.ActiveBarCount > 0)
            student.ActiveBarCount--;

        if (student.IsLocked && student.ActiveBarCount < 3)
        {
            student.IsLocked = false;
            student.UnlockedAt = DateTime.UtcNow;
        }

        await _studentRepository.UpdateAsync(student);
    }
}