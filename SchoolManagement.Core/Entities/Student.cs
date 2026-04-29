using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities;

[Table("STUDENT")]
public class Student
{
    [Key]
    [Column("student_no")]
    public string StudentNo { get; set; } = string.Empty;

    [Column("advisor_id")]
    public string? AdvisorId { get; set; }

    [Column("name_surname")]
    public string NameSurname { get; set; } = string.Empty;

    [Column("grade")]
    public string Grade { get; set; } = string.Empty;

    [Column("GPA", TypeName = "DECIMAL(3,2)")]
    public decimal GPA { get; set; }

    [Column("student_mail")]
    public string StudentMail { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    // Navigation Properties
    [ForeignKey("AdvisorId")]
    public virtual Advisor? Advisor { get; set; }

    [Column("active_bar_count")]
    public int ActiveBarCount { get; set; } = 0;

    [Column("is_locked")]               
    public bool IsLocked { get; set; } = false;

    [Column("locked_at")]
    public DateTime? LockedAt { get; set; }

    [Column("unlocked_at")]
    public DateTime? UnlockedAt { get; set; }

    public virtual ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
    public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    public virtual ICollection<Excuse> Excuses { get; set; } = new List<Excuse>();
}