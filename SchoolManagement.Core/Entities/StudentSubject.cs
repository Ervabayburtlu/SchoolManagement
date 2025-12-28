using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities;

[Table("STUDENT_SUBJECT")]
public class StudentSubject
{
    [Key]
    [Column("student_no_subject_id")]
    public string StudentNoSubjectId { get; set; } = string.Empty;

    [Column("subject_id")]
    public string SubjectId { get; set; } = string.Empty;

    [Column("student_no")]
    public string StudentNo { get; set; } = string.Empty;

    // Navigation Properties
    [ForeignKey("SubjectId")]
    public virtual Subject Subject { get; set; } = null!;

    [ForeignKey("StudentNo")]
    public virtual Student Student { get; set; } = null!;
}