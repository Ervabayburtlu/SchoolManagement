// ConsistencyService.cs
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

    // Mazeret yüklendi → bar değişmez, sadece onay/red etkiler
    public Task OnExcuseSubmittedAsync(string studentNo) => Task.CompletedTask;

    // Mazeret onaylandı → 1 bar sil
    public async Task OnExcuseApprovedAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);

        if (student.ActiveBarCount > 0)
            student.ActiveBarCount--;

        // Bar 3'ün altına düştüyse kilidi kaldır
        if (student.IsLocked && student.ActiveBarCount < 3)
        {
            student.IsLocked = false;
            student.UnlockedAt = DateTime.UtcNow;
        }

        await _studentRepository.UpdateAsync(student);
    }

    // Mazeret reddedildi → bar olduğu gibi kalır
    public Task OnExcuseRejectedAsync(string studentNo) => Task.CompletedTask;

    // Danışman kilidi açar → bar bir eksiltilir
    public async Task UnlockAccountAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);

        student.IsLocked = false;
        student.ActiveBarCount = Math.Max(0, student.ActiveBarCount - 1);  // ← 1 azalt, 0'ın altına düşme
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
            student.UnlockedAt = DateTime.UtcNow;
        }

        await _studentRepository.UpdateAsync(student);
    }

    // Bildirim yok (katılsa da katılmasa da) → bar ekle
    public async Task OnAbsentWithoutNotificationAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);
        if (student.IsLocked) return;

        await IncrementBarAsync(student);
    }

    // Bildirim gerçekleşmedi: katılacak dedi katılmadı VEYA katılmayacak dedi katıldı → bar ekle
    public async Task OnInconsistentBehaviorAsync(string studentNo)
    {
        var student = await GetStudentAsync(studentNo);
        if (student.IsLocked) return;

        await IncrementBarAsync(student);
    }

    public async Task<Student?> GetRecordAsync(string studentNo)
    {
        return await _studentRepository.GetByIdAsync(studentNo);
    }

    // Bar artırma ortak logic
    private async Task IncrementBarAsync(Student student)
    {
        student.ActiveBarCount++;

        if (student.ActiveBarCount >= 3)
        {
            student.IsLocked = true;
            student.LockedAt = DateTime.UtcNow;
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
}