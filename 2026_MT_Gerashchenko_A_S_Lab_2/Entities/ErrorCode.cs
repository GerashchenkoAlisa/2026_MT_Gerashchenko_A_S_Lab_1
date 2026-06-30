using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("ErrorCodes")]
public class ErrorCode : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ErrorCodeId { get; set; }

    public override int Id => this.ErrorCodeId;

    [Required]
    [MaxLength(20)]
    public string CodeValue { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? CodeDescription { get; set; }

    public virtual ICollection<BuildMessage> BuildMessages { get; } = [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.CodeValue} {val}".TrimEnd());
}