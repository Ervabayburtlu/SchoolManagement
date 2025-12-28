using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolManagement.Core.Entities;

[Table("ADVISOR")]
public class Advisor
{
    [Key]
    [Column("advisor_id")]
    public string AdvisorId { get; set; } = string.Empty;

    [Column("name_surname")]
    public string NameSurname { get; set; } = string.Empty;

    [Column("advisor_mail")]
    public string AdvisorMail { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    // Navigation Properties
    public virtual ICollection<Student> Students { get; set; } = new List<Student>();
}