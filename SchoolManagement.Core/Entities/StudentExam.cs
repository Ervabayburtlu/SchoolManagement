using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities;

[Table("STUDENT_EXAM")]
public class StudentExam
{
    [Key]
    [Column("student_no_exam_id")]
    public string StudentNoExamId { get; set; } = string.Empty;

    [Column("student_no")]
    public string StudentNo { get; set; } = string.Empty;

    [Column("exam_id")]
    public string ExamId { get; set; } = string.Empty;

    [Column("participation_status")]
    public string ParticipationStatus { get; set; } = string.Empty;

    [Column("consistency_note")]
    public string? ConsistencyNote { get; set; }

    // Navigation Properties
    [ForeignKey("StudentNo")]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("ExamId")]
    public virtual Exam Exam { get; set; } = null!;
}