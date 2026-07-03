using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("BuildExecutions")]
public class BuildExecution : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BuildExecutionId { get; set; }

    public override int Id => this.BuildExecutionId;

    [Required]
    public int ApplicationId { get; set; }

    [Required]
    public int ProcessStageId { get; set; }

    [Required]
    public int ExecutionResultId { get; set; }

    [Required]
    public DateTime ExecutionStartTime { get; set; }

    [Required]
    public long ExecutionTimeMs { get; set; }

    [Required]
    public int ExitCode { get; set; }

    [NotMapped]
    public int ErrorCount => this.BuildMessages?.Count(m => m.MessageSeverity?.SeverityName == "Error") ?? 0;

    [NotMapped]
    public int WarningCount => this.BuildMessages?.Count(m => m.MessageSeverity?.SeverityName == "Warning") ?? 0;

    [ForeignKey(nameof(ApplicationId))]
    public virtual Application Application { get; set; } = null!;

    [ForeignKey(nameof(ProcessStageId))]
    public virtual ProcessStage ProcessStage { get; set; } = null!;

    [ForeignKey(nameof(ExecutionResultId))]
    public virtual ExecutionResult ExecutionResult { get; set; } = null!;

    public virtual ICollection<BuildMessage> BuildMessages { get; } =[];

    public override string ToLogString(string val = "")
        => base.ToLogString($"[{this.ExecutionResult?.ResultName}] App={this.ApplicationId} Stage={this.ProcessStageId} {val}".TrimEnd());
}
