using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities;

[Table("BenchmarkTests")]
public class BenchmarkTest : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BenchmarkTestId { get; set; }

    public override int Id => this.BenchmarkTestId;

    [Required]
    [MaxLength(500)]
    public string TestDescription { get; set; } = string.Empty;

    public virtual ICollection<PerformanceMetric> PerformanceMetrics { get; } = [];

    public override string ToLogString(string val = "")
        => base.ToLogString($"{this.TestDescription} {val}".TrimEnd());
}