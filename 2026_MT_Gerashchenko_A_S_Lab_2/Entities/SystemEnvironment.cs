using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("SystemEnvironments")]
public class SystemEnvironment : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int SystemEnvironmentId { get; set; }

    public override int Id => this.SystemEnvironmentId;

    [Required]
    [MaxLength(200)]
    public string EnvironmentName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? EnvironmentDetails { get; set; }

    public virtual ICollection<ServerConfiguration> ServerConfigurations { get; } =[];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.EnvironmentName} {val}".TrimEnd());
}
