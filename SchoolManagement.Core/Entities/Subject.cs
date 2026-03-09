using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities;

[Table("SUBJECT")]
public class Subject
{
    [Key]
    [Column("subject_id")]
    public string SubjectId { get; set; } = string.Empty;

    [Column("subject_name")]
    public string SubjectName { get; set; } = string.Empty;

    [Column("academician_id")]
    public string? AcademicianId { get; set; }
    [Column("day_index")]
    public int DayIndex { get; set; }

    
 
    [Column("start_time")]
    public TimeSpan? StartTime { get; set; }

    [Column("end_time")]
    public TimeSpan? EndTime { get; set; }
    // Navigation Properties
    [ForeignKey("AcademicianId")]
    public virtual Academician? Academician { get; set; }

    public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
    public virtual ICollection<Exam> Exams { get; set; } = new List<Exam>();
}