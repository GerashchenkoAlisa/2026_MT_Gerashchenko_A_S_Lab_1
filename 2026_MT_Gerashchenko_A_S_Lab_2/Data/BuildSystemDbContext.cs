using Entities;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class BuildSystemDbContext : DbContext
{
    public BuildSystemDbContext(DbContextOptions<BuildSystemDbContext> options)
        : base(options)
    {
    }

    public BuildSystemDbContext()
    {
    }

    public DbSet<Application> Applications { get; set; }

    public DbSet<ProcessStage> ProcessStages { get; set; }

    public DbSet<BuildExecution> BuildExecutions { get; set; }

    public DbSet<BuildMessage> BuildMessages { get; set; }

    public DbSet<ErrorCode> ErrorCodes { get; set; }

    public DbSet<MessageSeverity> MessageSeverities { get; set; }

    public DbSet<ExecutionResult> ExecutionResults { get; set; }

    public DbSet<ProcessorModel> ProcessorModels { get; set; }

    public DbSet<ServerConfiguration> ServerConfigurations { get; set; }

    public DbSet<SystemEnvironment> SystemEnvironments { get; set; }

    public DbSet<BenchmarkTest> BenchmarkTests { get; set; }

    public DbSet<PerformanceMetric> PerformanceMetrics { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder?.Entity<Application>()
            .HasIndex(a => a.RepositoryPath)
            .IsUnique();

        modelBuilder?.Entity<ProcessStage>()
            .HasIndex(ps => ps.StageName)
            .IsUnique();

        modelBuilder?.Entity<BenchmarkTest>()
            .HasIndex(bt => bt.TestDescription)
            .IsUnique();

        modelBuilder?.Entity<ProcessorModel>()
            .HasIndex(pm => pm.ProcessorName)
            .IsUnique();

        modelBuilder?.Entity<MessageSeverity>()
            .HasIndex(ms => ms.SeverityName)
            .IsUnique();

        modelBuilder?.Entity<ExecutionResult>()
            .HasIndex(er => er.ResultName)
            .IsUnique();

        modelBuilder?.Entity<ErrorCode>()
            .HasIndex(ec => ec.CodeValue)
            .IsUnique();

        modelBuilder?.Entity<SystemEnvironment>()
            .HasIndex(se => se.EnvironmentName).IsUnique();
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(DatabaseConfig.ConnectionString);
        }
    }
}