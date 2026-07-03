using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using Microsoft.EntityFrameworkCore;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Data;

public class BuildSystemDbContext(DbContextOptions<BuildSystemDbContext> options) : DbContext(options)
{
    public DbSet<Application> Applications { get; set; } = null!;

    public DbSet<BuildExecution> BuildExecutions { get; set; } = null!;

    public DbSet<BuildMessage> BuildMessages { get; set; } = null!;

    public DbSet<BenchmarkTest> BenchmarkTests { get; set; } = null!;

    public DbSet<ErrorCode> ErrorCodes { get; set; } = null!;

    public DbSet<ExecutionResult> ExecutionResults { get; set; } = null!;

    public DbSet<MessageSeverity> MessageSeverities { get; set; } = null!;

    public DbSet<PerformanceMetric> PerformanceMetrics { get; set; } = null!;

    public DbSet<ProcessStage> ProcessStages { get; set; } = null!;

    public DbSet<ProcessorModel> ProcessorModels { get; set; } = null!;

    public DbSet<ServerConfiguration> ServerConfigurations { get; set; } = null!;

    public DbSet<SystemEnvironment> SystemEnvironments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
