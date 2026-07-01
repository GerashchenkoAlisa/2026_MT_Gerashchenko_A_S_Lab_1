using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

[Table("PerformanceMetrics")]
public class PerformanceMetric : BaseEntity<int>
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int PerformanceMetricId { get; set; }

    public override int Id => this.PerformanceMetricId;

    [Required]
    public int BenchmarkTestId { get; set; }

    [Required]
    public int ServerConfigurationId { get; set; }

    [Required]
    public int BuildExecutionId { get; set; }

    [Required]
    public long SingleThreadTimeMs { get; set; }

    [Required]
    public long MultiThreadTimeMs { get; set; }

    [NotMapped]
    public decimal PerformanceGain =>
        this.MultiThreadTimeMs == 0 ? 0 : (decimal)this.SingleThreadTimeMs / this.MultiThreadTimeMs;

    [NotMapped]
    public long TotalTimeMs => this.SingleThreadTimeMs + this.MultiThreadTimeMs;

    [Required]
    public DateTime MetricRecordTime { get; set; }

    [ForeignKey(nameof(BenchmarkTestId))]
    public virtual BenchmarkTest BenchmarkTest { get; set; } = null!;

    [ForeignKey(nameof(ServerConfigurationId))]
    public virtual ServerConfiguration ServerConfiguration { get; set; } = null!;

    [ForeignKey(nameof(BuildExecutionId))]
    public virtual BuildExecution BuildExecution { get; set; } = null!;

    public override string ToLogString(string val = "")
        => base.ToLogString(
            $"Single={this.SingleThreadTimeMs}ms Multi={this.MultiThreadTimeMs}ms Gain={this.PerformanceGain:F4}x {val}".TrimEnd());
}
