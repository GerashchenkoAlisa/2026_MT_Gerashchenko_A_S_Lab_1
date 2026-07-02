using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using _2026_MT_Gerashchenko_A_S_Lab_2.UnitsOfWork;
using _2026_MT_Gerashchenko_A_S_Lab_4.Infrastructure;
using _2026_MT_Gerashchenko_A_S_Lab_4.Models;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace _2026_MT_Gerashchenko_A_S_Lab_4.Persistence;
public sealed class MetricsPersistenceService(IBuildSystemUnitOfWork uow, BuildSystemDbContext dbContext)
{
    internal class PipelineStepExecution
    {
        public int ProjectId { get; set; }
        public int StageTypeId { get; set; }
        public int ExecutionStatusId { get; set; }
        public DateTime StartedAt { get; set; }
        public int DurationMs { get; set; }
        public int ExitCode { get; set; }
    }

    private const int DefaultStageTypeId = 4;
    private const int DefaultExecutionStatusId = 1;

    public async Task SaveRunsAsync(
        IReadOnlyList<ScanRunResult> runs,
        EnvironmentInfo env,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(runs);
        ArgumentNullException.ThrowIfNull(env);

        await dbContext.Database.MigrateAsync(ct).ConfigureAwait(false);

        var host = await GetOrCreateHostAsync(uow, env).ConfigureAwait(false);
        var step = await GetOrCreateStepAsync(uow).ConfigureAwait(false);

        foreach (var run in runs)
        {
            var testDescription = BuildTestDescription(run);
            var perfTest = await GetOrCreatePerformanceTestAsync(uow, testDescription).ConfigureAwait(false);

            var alreadyExists = false;

            if (alreadyExists)
                continue;

            var seqMs = (long)run.TotalElapsed.TotalMilliseconds;

            var metric = new PerformanceMetric
            {
                BenchmarkTestId = perfTest.BenchmarkTestId,
                ServerConfigurationId = host.ServerConfigurationId,
                BuildExecutionId = 1,
                SingleThreadTimeMs = seqMs,
                MultiThreadTimeMs = Math.Max(1, seqMs / Math.Max(1, run.MaxParallelism)),
                MetricRecordTime = DateTime.UtcNow,
            };

            await uow.PerformanceMetrics.AddAsync(metric).ConfigureAwait(false);
        }

        await uow.SaveChangesAsync().ConfigureAwait(false);
    }

    private static string BuildTestDescription(ScanRunResult run) =>
        run.OperationType switch
        {
            OperationType.Scan => $"HttpScanner:Scan:p{run.MaxParallelism}",
            OperationType.Download => $"HttpScanner:Download:p{run.MaxParallelism}",
            _ => "Unknown"
        };

    private static async Task<ServerConfiguration> GetOrCreateHostAsync(IBuildSystemUnitOfWork uow, EnvironmentInfo env)
    {
        var servers = await uow.ServerConfigurations.GetAllAsync().ConfigureAwait(false);
        var existing = servers.FirstOrDefault();

        if (existing is not null)
            return existing;

        var server = new ServerConfiguration
        {
            ProcessorModelId = 1,
            MemoryCapacityGb = env.RamGb,
            SystemEnvironmentId = 1,
        };

        await uow.ServerConfigurations.AddAsync(server).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);
        return server;
    }

    private static async Task<PipelineStepExecution> GetOrCreateStepAsync(IBuildSystemUnitOfWork uow)
    {
        var step = new PipelineStepExecution
        {
            ProjectId = 1,
            StageTypeId = DefaultStageTypeId,
            ExecutionStatusId = DefaultExecutionStatusId,
            StartedAt = DateTime.UtcNow,
            DurationMs = 0,
            ExitCode = 0,
        };

        await uow.SaveChangesAsync().ConfigureAwait(false);
        return step;
    }

    private static async Task<BenchmarkTest> GetOrCreatePerformanceTestAsync(IBuildSystemUnitOfWork uow, string description)
    {
        var existing = await uow.BenchmarkTests.GetByTestDescriptionAsync(description).ConfigureAwait(false);
        if (existing is not null)
            return existing;

        var test = new BenchmarkTest { TestDescription = description };
        await uow.BenchmarkTests.AddAsync(test).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);
        return test;
    }
}