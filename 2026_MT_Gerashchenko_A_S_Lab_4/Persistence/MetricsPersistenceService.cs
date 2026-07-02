namespace _2026_MT_Gerashchenko_A_S_Lab_4.Persistence;

using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using _2026_MT_Gerashchenko_A_S_Lab_4.Models;
using _2026_MT_Gerashchenko_A_S_Lab_2.UnitsOfWork;

public sealed class MetricsPersistenceService(IUnitOfWork uow, ApplicationDbContext dbContext)
{
    private const int DefaultStageTypeId = 4;
    private const int DefaultExecutionStatusId = 1;
    private const int DefaultStatusId = 2;  
    private readonly IUnitOfWork uow = uow;
    private readonly ApplicationDbContext dbContext = dbContext;

    public async Task SaveRunsAsync(
        IReadOnlyList<ScanRunResult> runs,
        EnvironmentInfo env,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(runs);
        ArgumentNullException.ThrowIfNull(env);

        await this.dbContext.Database.MigrateAsync(ct).ConfigureAwait(false);

        var host = await GetOrCreateHostAsync(this.uow, env).ConfigureAwait(false);
        var step = await GetOrCreateStepAsync(this.uow).ConfigureAwait(false);

        foreach (var run in runs)
        {
            var testDescription = BuildTestDescription(run);
            var perfTest = await GetOrCreatePerformanceTestAsync(this.uow, testDescription).ConfigureAwait(false);

            var alreadyExists = (await this.uow.ThreadSpeedMetrics
                .GetByPerformanceTestIdAsync(perfTest.Id)
                .ConfigureAwait(false))
                .Any(m => m.HostId == host.Id);

            if (alreadyExists)
            {
                continue;
            }

            var seqMs = (long)run.TotalElapsed.TotalMilliseconds;

            var metric = new ThreadSpeedMetric
            {
                PerformanceTestId = perfTest.Id,
                HostId = host.Id,
                PipelineStepExecutionId = step.Id,
                SequentialTimeMs = seqMs,
                ParallelTimeMs = Math.Max(1, seqMs / Math.Max(1, run.MaxParallelism)),
                StartedAt = DateTime.UtcNow,
            };

            await this.uow.ThreadSpeedMetrics.AddAsync(metric).ConfigureAwait(false);
        }

        await this.uow.SaveChangesAsync().ConfigureAwait(false);
    }

    private static string BuildTestDescription(ScanRunResult run) =>
        run.OperationType switch
        {
            OperationType.Scan => $"HttpScanner:Scan:p{run.MaxParallelism}",
            OperationType.Download => $"HttpScanner:Download:p{run.MaxParallelism}",
            _ => throw new ArgumentOutOfRangeException(nameof(run), run.OperationType, "Unknown OperationType"),
        };

    private static async Task<Entities.Host> GetOrCreateHostAsync(IUnitOfWork uow, EnvironmentInfo env)
    {
        var hosts = await uow.Hosts.GetAllAsync().ConfigureAwait(false);
        var existing = hosts.FirstOrDefault(h =>
            h.RamGb == env.RamGb &&
            h.CpuModel?.ModelName != null &&
            env.CpuModel.Contains(h.CpuModel.ModelName, StringComparison.OrdinalIgnoreCase));

        if (existing is not null)
        {
            return existing;
        }

        var osTypes = await uow.OperatingSystemTypes.GetAllAsync().ConfigureAwait(false);
        var osType = osTypes.FirstOrDefault(o =>
            env.OsDescription.Contains(o.Name, StringComparison.OrdinalIgnoreCase));

        if (osType is null)
        {
            osType = new OperatingSystemType
            {
                Name = env.OsDescription[..Math.Min(200, env.OsDescription.Length)],
            };
            await uow.OperatingSystemTypes.AddAsync(osType).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        var cpuModels = await uow.CpuModels.GetAllAsync().ConfigureAwait(false);
        var cpu = cpuModels.FirstOrDefault(c =>
            c.ModelName.Contains(env.CpuModel, StringComparison.OrdinalIgnoreCase));

        if (cpu is null)
        {
            cpu = new CpuModel
            {
                ModelName = env.CpuModel[..Math.Min(200, env.CpuModel.Length)],
                PhysicalCoreCount = env.PhysicalCoreCount,
                LogicalThreadCount = env.LogicalThreadCount,
            };
            await uow.CpuModels.AddAsync(cpu).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        var host = new Entities.Host
        {
            CpuModelId = cpu.Id,
            RamGb = env.RamGb,
            OperatingSystemTypeId = osType.Id,
        };

        await uow.Hosts.AddAsync(host).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);
        return host;
    }

    private static async Task<PipelineStepExecution> GetOrCreateStepAsync(IUnitOfWork uow)
    {
        var projects = await uow.Projects.GetAllAsync().ConfigureAwait(false);
        var project = projects.FirstOrDefault(p => p.Name == "HttpScannerLab4");

        if (project is null)
        {
            project = new Project
            {
                Name = "HttpScannerLab4",
                FolderPath = "/lab4/HttpScanner",
            };
            await uow.Projects.AddAsync(project).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);
        }

        var step = new PipelineStepExecution
        {
            ProjectId = project.Id,
            StageTypeId = DefaultStageTypeId,
            ExecutionStatusId = DefaultExecutionStatusId,
            StartedAt = DateTime.UtcNow,
            DurationMs = 0,
            ExitCode = 0,
        };

        await uow.PipelineStepExecutions.AddAsync(step).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);
        return step;
    }

    private static async Task<PerformanceTest> GetOrCreatePerformanceTestAsync(IUnitOfWork uow, string description)
    {
        var existing = await uow.PerformanceTests.GetByDescriptionAsync(description).ConfigureAwait(false);
        if (existing is not null)
        {
            return existing;
        }

        var test = new PerformanceTest { Description = description };
        await uow.PerformanceTests.AddAsync(test).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);
        return test;
    }
}