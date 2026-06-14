using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using Microsoft.EntityFrameworkCore;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Data;

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

        modelBuilder?.Entity<ProcessStage>().HasData(
            new ProcessStage { ProcessStageId = 1, StageName = "Compile" },
            new ProcessStage { ProcessStageId = 2, StageName = "UnitTest" },
            new ProcessStage { ProcessStageId = 3, StageName = "CodeAnalysis" },
            new ProcessStage { ProcessStageId = 4, StageName = "Deploy" });

        modelBuilder?.Entity<ProcessorModel>().HasData(
            new ProcessorModel { ProcessorModelId = 1, ProcessorName = "AMD Ryzen 9 7950X", PhysicalCores = 16, LogicalCores = 32 },
            new ProcessorModel { ProcessorModelId = 2, ProcessorName = "Intel Core i9-13900K", PhysicalCores = 24, LogicalCores = 32 },
            new ProcessorModel { ProcessorModelId = 3, ProcessorName = "AMD Ryzen 5 5600X", PhysicalCores = 6, LogicalCores = 12 });

        modelBuilder?.Entity<MessageSeverity>().HasData(
            new MessageSeverity { MessageSeverityId = 1, SeverityName = "Error", SeverityDescription = "Critical compilation error" },
            new MessageSeverity { MessageSeverityId = 2, SeverityName = "Warning", SeverityDescription = "Non-blocking issue" },
            new MessageSeverity { MessageSeverityId = 3, SeverityName = "Info", SeverityDescription = "Informational notification" });

        modelBuilder?.Entity<ExecutionResult>().HasData(
            new ExecutionResult { ExecutionResultId = 1, ResultName = "Passed", ResultDescription = "Execution completed successfully" },
            new ExecutionResult { ExecutionResultId = 2, ResultName = "Failed", ResultDescription = "Execution encountered errors" },
            new ExecutionResult { ExecutionResultId = 3, ResultName = "Aborted", ResultDescription = "Execution was aborted" },
            new ExecutionResult { ExecutionResultId = 4, ResultName = "InProgress", ResultDescription = "Execution is currently running" });

        modelBuilder?.Entity<SystemEnvironment>().HasData(
            new SystemEnvironment { SystemEnvironmentId = 1, EnvironmentName = "Windows 11 Pro (64-bit)" },
            new SystemEnvironment { SystemEnvironmentId = 2, EnvironmentName = "Windows 10 Pro (64-bit)" },
            new SystemEnvironment { SystemEnvironmentId = 3, EnvironmentName = "Ubuntu 24.04 LTS (64-bit)" },
            new SystemEnvironment { SystemEnvironmentId = 4, EnvironmentName = "macOS Sequoia 15" });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite(DatabaseConfig.ConnectionString);
        }
    }
}