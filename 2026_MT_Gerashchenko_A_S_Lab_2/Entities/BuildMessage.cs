using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("BuildMessages")]
public class BuildMessage : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BuildMessageId { get; set; }

    public override int Id => this.BuildMessageId;

    [Required]
    public int BuildExecutionId { get; set; }

    [Required]
    public DateTime MessageTimestamp { get; set; }

    [Required]
    public int MessageSeverityId { get; set; }

    [ForeignKey(nameof(MessageSeverityId))]
    public virtual MessageSeverity MessageSeverity { get; set; } = null!;

    public int? ErrorCodeId { get; set; }

    [ForeignKey(nameof(ErrorCodeId))]
    public virtual ErrorCode? ErrorCode { get; set; }

    [Required]
    public string MessageText { get; set; } = string.Empty;

    [ForeignKey(nameof(BuildExecutionId))]
    public virtual BuildExecution BuildExecution { get; set; } = null!;

    public override string ToLogString(string val = "")
        => base.ToLogString($"[{this.MessageSeverity?.SeverityName}] {this.ErrorCode?.CodeValue}: {this.MessageText} {val}".TrimEnd());
}