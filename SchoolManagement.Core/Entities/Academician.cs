using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities;

[Table("ACADEMICIAN")]
public class Academician
{
    [Key]
    [Column("academician_id")]
    public string AcademicianId { get; set; } = string.Empty;

    [Column("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [Column("last_name")]
    public string LastName { get; set; } = string.Empty;

    [Column("academician_email")]
    public string AcademicianEmail { get; set; } = string.Empty;

    [Column("academician_phone")]
    public string AcademicianPhone { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    // Refresh Token
    [Column("refresh_token")]
    public string? RefreshToken { get; set; }

    [Column("refresh_token_expiry")]
    public DateTime? RefreshTokenExpiry { get; set; }

    // Navigation Properties
    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}