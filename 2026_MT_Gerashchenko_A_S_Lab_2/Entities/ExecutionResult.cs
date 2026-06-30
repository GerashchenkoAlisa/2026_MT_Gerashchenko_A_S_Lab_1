using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;


[Table("ExecutionResults")]
public class ExecutionResult : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ExecutionResultId { get; set; }

    public override int Id => this.ExecutionResultId;

    [Required]
    [MaxLength(30)]
    public string ResultName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? ResultDescription { get; set; }

    public virtual ICollection<BuildExecution> BuildExecutions { get; } = [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.ResultName} {val}".TrimEnd());
}