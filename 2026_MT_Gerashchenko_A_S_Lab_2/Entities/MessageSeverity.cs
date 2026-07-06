using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

[Table("MessageSeverities")]
public class MessageSeverity : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MessageSeverityId { get; set; }

    public override int Id => this.MessageSeverityId;

    [Required]
    [MaxLength(20)]
    public string SeverityName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? SeverityDescription { get; set; }

    public virtual ICollection<BuildMessage> BuildMessages { get; } =
        [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.SeverityName} {val}".TrimEnd());
}