using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("ServerConfigurations")]
public class ServerConfiguration : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ServerConfigurationId { get; set; }

    public override int Id => this.ServerConfigurationId;

    public int? ProcessorModelId { get; set; }

    [Required]
    [Column(TypeName = "decimal(5,2)")]
    public decimal MemoryCapacityGb { get; set; }

    [Required]
    public int SystemEnvironmentId { get; set; }

    [ForeignKey(nameof(ProcessorModelId))]
    public virtual ProcessorModel? ProcessorModel { get; set; }

    [ForeignKey(nameof(SystemEnvironmentId))]
    public virtual SystemEnvironment SystemEnvironment { get; set; } = null!;

    public virtual ICollection<PerformanceMetric> PerformanceMetrics { get; } =[];

    public override string ToLogString(string val = "")
    => base.ToLogString(
        $"{this.SystemEnvironment?.EnvironmentName ?? "Unknown"} RAM={this.MemoryCapacityGb}GB {val}".TrimEnd());
}
