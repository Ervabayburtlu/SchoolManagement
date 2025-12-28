using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities;

[Table("EXCUSE")]
public class Excuse
{
    [Key]
    [Column("excuse_id")]
    public string ExcuseId { get; set; } = string.Empty;

    [Column("student_no")]
    public string StudentNo { get; set; } = string.Empty;

    [Column("advisor_id")]
    public string? AdvisorId { get; set; }

    [Column("exam_id")]
    public string? ExamId { get; set; }

    [Column("excuse_description")]
    public string ExcuseDescription { get; set; } = string.Empty;

    [Column("request_date")]
    public DateTime RequestDate { get; set; }

    [Column("response_date")]
    public DateTime? ResponseDate { get; set; }

    [Column("document_path")]
    public string? DocumentPath { get; set; }

    [Column("status")]
    public string Status { get; set; } = "PENDING"; // PENDING, APPROVED, REJECTED

    // Navigation Properties
    [ForeignKey("StudentNo")]
    public virtual Student Student { get; set; } = null!;

    [ForeignKey("AdvisorId")]
    public virtual Advisor? Advisor { get; set; }
}