using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("ProcessStages")]
public class ProcessStage : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ProcessStageId { get; set; }

    public override int Id => this.ProcessStageId;

    [Required]
    [MaxLength(50)]
    public string StageName { get; set; } = string.Empty;

    public virtual ICollection<BuildExecution> BuildExecutions { get; } = [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.StageName} {val}".TrimEnd());
}