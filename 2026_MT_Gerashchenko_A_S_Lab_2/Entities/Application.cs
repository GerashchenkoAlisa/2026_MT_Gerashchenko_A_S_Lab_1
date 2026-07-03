using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("Applications")]
public class Application : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ApplicationId { get; set; }

    public override int Id => this.ApplicationId;

    [Required]
    [MaxLength(200)]
    public string ApplicationName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string RepositoryPath { get; set; } = string.Empty;

    public virtual ICollection<BuildExecution> BuildExecutions { get; } =[];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.ApplicationName} @ {this.RepositoryPath} {val}".TrimEnd());
}
