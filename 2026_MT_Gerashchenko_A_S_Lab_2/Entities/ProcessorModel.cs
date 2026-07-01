using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("ProcessorModels")]
public class ProcessorModel : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProcessorModelId { get; set; }

    public override int Id => this.ProcessorModelId;

    [Required]
    [MaxLength(200)]
    public string ProcessorName { get; set; } = string.Empty;

    [Required]
    public int PhysicalCores { get; set; }

    [Required]
    public int LogicalCores { get; set; }

    public virtual ICollection<ServerConfiguration> ServerConfigurations { get; } = [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.ProcessorName} Cores={this.PhysicalCores} Threads={this.LogicalCores} {val}".TrimEnd());
}