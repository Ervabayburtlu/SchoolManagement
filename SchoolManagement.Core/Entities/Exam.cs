using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities;

[Table("EXAM")]
public class Exam
{
    [Key]
    [Column("exam_id")]
    public string ExamId { get; set; } = string.Empty;

    [Column("subject_id")]
    public string SubjectId { get; set; } = string.Empty;

    [Column("exam_type")]
    public string ExamType { get; set; } = string.Empty;

    [Column("exam_date")]
    public DateTime ExamDate { get; set; }

    [Column("exam_description")]
    public string? ExamDescription { get; set; }

    // Navigation Properties
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;

    public virtual ICollection<StudentExam> StudentExams { get; set; } = new List<StudentExam>();
}