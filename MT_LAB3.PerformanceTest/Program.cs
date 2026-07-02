using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MT_LAB3.MatrixLib;
using System.Diagnostics;
using System.Numerics;
using _2026_MT_Gerashchenko_A_S_Lab_2.UnitsOfWork;
using _2026_MT_Gerashchenko_A_S_Lab_2.Repositories;
using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

namespace MT_LAB3.PerformanceTest;
internal sealed record BenchResult(
    int Size,
    string TestType,
    string Algorithm,
    string Storage,
    long Microseconds);

internal static class Program
{
    private static readonly List<BenchResult> Results = new();

    private static string _cpuModelName = "Unknown CPU";
    private static int _physCores = Environment.ProcessorCount / 2;
    private static int _logCores = Environment.ProcessorCount;
    private static decimal _ramGb = 0m;
    private static string _osName = Environment.OSVersion.ToString();

    private static string ResolveDbPath()
    {
        string binDir = AppContext.BaseDirectory;
        string projDir = Path.GetFullPath(Path.Combine(binDir, "..", "..", ".."));
        string slnDir = Path.GetFullPath(Path.Combine(projDir, ".."));

        string[] candidates =
        {
            Path.Combine(slnDir, "2026_MT_Gerashchenko_A_S_Lab_2",
                         "2026_MT_Gerashchenko_A_S_Lab_2", "app.db"),
            Path.Combine(slnDir, "..", "2026_MT_Gerashchenko_A_S_Lab_2",
                         "2026_MT_Gerashchenko_A_S_Lab_2", "app.db"),
            Path.Combine(slnDir, "..", "2026_MT_Gerashchenko_A_S_Lab_2",
                         "2026_MT_Gerashchenko_A_S_Lab_2", "app.db"),
        };

        foreach (string c in candidates)
        {
            string full = Path.GetFullPath(c);
            if (File.Exists(full))
                return full;
        }

        return Path.GetFullPath(candidates[0]);
    }

    static async Task Main()
    {
        PrintBanner();
        CollectSystemInfo();
        PrintSystemInfo();

        foreach (int size in new[] { 500, 2000 })
        {
            Console.WriteLine($"\nBenchmarking {size}x{size} matrices...");
            Console.WriteLine(new string('-', 60));
            RunBenchmarkForSize(size);
        }

        FindBreakevenPoint();
        PrintSummary();
        await SaveResultsToDatabaseAsync();

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void CollectSystemInfo()
    {
        _cpuModelName = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")
                        ?? Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE")
                        ?? "Unknown CPU";
    }

    static void PrintSystemInfo()
    {
        Console.WriteLine("\nSYSTEM INFORMATION");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"   CPU  : {_cpuModelName}");
        Console.WriteLine($"   Cores: {_physCores} physical / {_logCores} logical");
        Console.WriteLine($"   RAM  : {(_ramGb > 0 ? $"{_ramGb} GB" : "unknown")}");
        Console.WriteLine($"   OS   : {_osName}");
    }

    static void RunBenchmarkForSize(int size)
    {
        var rng = new Random(1);
        var (rA, rB) = MakePair<int>(size, rng, (r, c) => new RectMatrix<int>(r, c));
        var (jA, jB) = MakePair<int>(size, rng, (r, c) => new JaggedMatrix<int>(r, c));
        var (fA, fB) = MakePair<int>(size, rng, (r, c) => new FlatMatrix<int>(r, c));

        var triples = new (string name, IMatrix<int> a, IMatrix<int> b)[]
        {
            ("RectMatrix",   rA, rB),
            ("JaggedMatrix", jA, jB),
            ("FlatMatrix",   fA, fB),
        };

        foreach (var (name, a, b) in triples)
        {
            BenchmarkAddition(a, b, name, size);

            if (size <= 500)
                BenchmarkMultiplication(a, b, name, size);
            else
                Console.WriteLine($"   {name}: skipping multiplication for {size}x{size}");
        }
    }

    static void BenchmarkAddition(IMatrix<int> a, IMatrix<int> b, string storage, int size)
    {
        Measure("Addition", "AddByRowsSequential", storage, size, () => a.AddByRowsSequential(b));
        Measure("Addition", "AddByRowsParallel", storage, size, () => a.AddByRowsParallel(b));
        Measure("Addition", "AddByColumnsSequential", storage, size, () => a.AddByColumnsSequential(b));
        Measure("Addition", "AddByColumnsParallel", storage, size, () => a.AddByColumnsParallel(b));
    }

    static void BenchmarkMultiplication(IMatrix<int> a, IMatrix<int> b, string storage, int size)
    {
        Measure("Multiply", "MultiplySequential", storage, size, () => a.MultiplySequential(b));
        Measure("Multiply", "MultiplyParallel", storage, size, () => a.MultiplyParallel(b));
        Measure("Multiply", "MultiplyOptimSequential", storage, size, () => a.MultiplyOptimSequential(b));
        Measure("Multiply", "MultiplyOptimParallel", storage, size, () => a.MultiplyOptimParallel(b));
        Measure("Multiply", "MultiplyNaiveSequential", storage, size, () => a.MultiplyNaiveSequential(b));
        Measure("Multiply", "MultiplyNaiveParallel", storage, size, () => a.MultiplyNaiveParallel(b));
    }

    static void Measure(string testType, string algorithm, string storage, int size, Action action)
    {
        const int Warmup = 1;
        const int Runs = 3;

        for (int w = 0; w < Warmup; w++) action();

        var times = new long[Runs];
        for (int r = 0; r < Runs; r++)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            times[r] = sw.ElapsedTicks * 1_000_000L / Stopwatch.Frequency;
        }

        Array.Sort(times);
        long median = times[Runs / 2];

        Results.Add(new BenchResult(size, testType, algorithm, storage, median));
        Console.WriteLine($"   {storage,-14} | {algorithm,-25} | {median,8} us");
    }

    static void FindBreakevenPoint()
    {
        Console.WriteLine("\nBREAKEVEN ANALYSIS - AddByRows parallel vs sequential");
        Console.WriteLine(new string('-', 65));

        int[] sizes = { 10, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
        const int Runs = 5;
        const int ConsecutiveReq = 3;

        int wins = 0;
        int? breakevenN = null;
        var rng = new Random(7);

        foreach (int size in sizes)
        {
            var (a, b) = MakePair<int>(size, rng, (r, c) => new RectMatrix<int>(r, c));

            long seqUs = MedianMicros(Runs, () => a.AddByRowsSequential(b));
            long parUs = MedianMicros(Runs, () => a.AddByRowsParallel(b));
            double ratio = (double)seqUs / Math.Max(1, parUs);

            string mark = parUs < seqUs ? "parallel faster" : "sequential faster";
            Console.WriteLine(
                $"   {size,5}x{size,-5} | seq={seqUs,7} us | par={parUs,7} us | ratio={ratio:F2} | {mark}");

            if (parUs < seqUs)
            {
                wins++;
                if (wins >= ConsecutiveReq && breakevenN is null)
                {
                    breakevenN = size;
                    Console.WriteLine($"\n   STABLE BREAKEVEN at {size}x{size} ({ConsecutiveReq} consecutive parallel wins)\n");
                }
            }
            else
            {
                wins = 0;
            }
        }

        if (breakevenN is null)
            Console.WriteLine("\n   No stable breakeven found within tested sizes.");
    }

    static long MedianMicros(int runs, Action action)
    {
        action();
        var times = new long[runs];
        for (int i = 0; i < runs; i++)
        {
            var sw = Stopwatch.StartNew();
            action();
            sw.Stop();
            times[i] = sw.ElapsedTicks * 1_000_000L / Stopwatch.Frequency;
        }
        Array.Sort(times);
        return times[runs / 2];
    }

    static void PrintSummary()
    {
        Console.WriteLine("\nPERFORMANCE SUMMARY (median us, averaged across storage types)");
        Console.WriteLine(new string('-', 72));

        foreach (var g in Results
            .GroupBy(r => new { r.Size, r.TestType, r.Algorithm })
            .OrderBy(g => g.Key.Size)
            .ThenBy(g => g.Key.TestType)
            .ThenBy(g => g.Key.Algorithm))
        {
            double avg = g.Average(r => r.Microseconds);
            Console.WriteLine($"   {g.Key.Size,5}x{g.Key.Size} | {g.Key.Algorithm,-25} | {avg,8:F0} us");
        }

        Console.WriteLine("\nFASTEST MULTIPLICATION PER SIZE:");
        foreach (var sg in Results
            .Where(r => r.TestType == "Multiply")
            .GroupBy(r => r.Size)
            .OrderBy(g => g.Key))
        {
            var best = sg.MinBy(r => r.Microseconds)!;
            Console.WriteLine($"   {best.Size,5}x{best.Size} -> {best.Algorithm} ({best.Storage}) - {best.Microseconds} us");
        }
    }

    static async Task SaveResultsToDatabaseAsync()
    {
        string dbPath = ResolveDbPath();
        string tmpDb = Path.Combine(Path.GetTempPath(), $"lab3_{Guid.NewGuid():N}.db");
        Console.WriteLine($"\nSaving results to database: {dbPath}");

        File.Copy(dbPath, tmpDb, overwrite: true);

        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(opts =>
            opts.UseSqlite($"Data Source={tmpDb}"));
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        bool success = false;
        await using var sp = services.BuildServiceProvider();
        await using var scope = sp.CreateAsyncScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        try
        {
            var cpu = (await uow.CpuModels.FindAsync(c => c.ModelName == _cpuModelName))
                      .FirstOrDefault()
                      ?? new CpuModel
                      {
                          ModelName = _cpuModelName,
                          PhysicalCoreCount = _physCores,
                          LogicalThreadCount = _logCores,
                      };
            if (cpu.Id == 0)
            {
                await uow.CpuModels.AddAsync(cpu);
                await uow.SaveChangesAsync();
            }

            var osType = (await uow.OperatingSystemTypes.FindAsync(o => o.Name == _osName))
                         .FirstOrDefault()
                         ?? new OperatingSystemType { Name = _osName };
            if (osType.Id == 0)
            {
                await uow.OperatingSystemTypes.AddAsync(osType);
                await uow.SaveChangesAsync();
            }

            var host = (await uow.Hosts.FindAsync(
                            h => h.CpuModelId == cpu.Id && h.OperatingSystemTypeId == osType.Id))
                       .FirstOrDefault()
                       ?? new Host
                       {
                           CpuModelId = cpu.Id,
                           RamGb = _ramGb > 0 ? _ramGb : 0m,
                           OperatingSystemTypeId = osType.Id,
                       };
            if (host.Id == 0)
            {
                await uow.Hosts.AddAsync(host);
                await uow.SaveChangesAsync();
            }

            string description = $"MatrixLib Lab3 {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC";
            var perfTest = new Entities.PerformanceTest { Description = description };
            await uow.PerformanceTests.AddAsync(perfTest);
            await uow.SaveChangesAsync();

            var defaultProject = (await uow.Projects.GetAllAsync()).FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "No projects in Lab-2 DB. Run the Lab-2 program first to seed data.");

            var stageTypes = await uow.StageTypes.GetAllAsync();
            var execStatuses = await uow.FindAsync<Entities.ExecutionStatus>();
            var runStage = stageTypes.First(s => s.Name == "Run");
            var successStatus = execStatuses.First(s => s.Name == "Success");

            foreach (var seqResult in Results.Where(r => !r.Algorithm.Contains("Parallel")))
            {
                string parallelAlgo = seqResult.Algorithm.Replace("Sequential", "Parallel");
                var parResult = Results.FirstOrDefault(
                    r => r.Size == seqResult.Size
                      && r.TestType == seqResult.TestType
                      && r.Algorithm == parallelAlgo
                      && r.Storage == seqResult.Storage);

                long parUs = parResult?.Microseconds ?? seqResult.Microseconds;

                var step = new PipelineStepExecution
                {
                    ProjectId = defaultProject.Id,
                    StageTypeId = runStage.Id,
                    ExecutionStatusId = successStatus.Id,
                    StartedAt = DateTime.UtcNow,
                    DurationMs = seqResult.Microseconds / 1000 + parUs / 1000,
                    ExitCode = 0,
                };
                await uow.PipelineStepExecutions.AddAsync(step);
                await uow.SaveChangesAsync();

                var metric = new ThreadSpeedMetric
                {
                    PerformanceTestId = perfTest.Id,
                    HostId = host.Id,
                    PipelineStepExecutionId = step.Id,
                    SequentialTimeMs = seqResult.Microseconds / 1000,
                    ParallelTimeMs = parUs / 1000,
                    StartedAt = DateTime.UtcNow,
                };
                await uow.ThreadSpeedMetrics.AddAsync(metric);
            }

            await uow.SaveChangesAsync();
            success = true;

            int saved = Results.Count(r => !r.Algorithm.Contains("Parallel"));
            Console.WriteLine($"Saved {saved} ThreadSpeedMetric rows to database.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DB save failed: {ex.Message}");
        }
        finally
        {
            if (success)
            {
                File.Copy(tmpDb, dbPath, overwrite: true);
                Console.WriteLine("Database updated successfully.");
            }
            if (File.Exists(tmpDb)) File.Delete(tmpDb);
            string tmpWal = tmpDb + "-wal";
            string tmpShm = tmpDb + "-shm";
            if (File.Exists(tmpWal)) File.Delete(tmpWal);
            if (File.Exists(tmpShm)) File.Delete(tmpShm);
        }
    }

    static (IMatrix<T> a, IMatrix<T> b) MakePair<T>(
        int size, Random rng, Func<int, int, IMatrix<T>> factory)
        where T : INumber<T>
    {
        var a = factory(size, size);
        var b = factory(size, size);
        a.Fill((_, _) => T.CreateChecked(rng.Next(1, 100)));
        b.Fill((_, _) => T.CreateChecked(rng.Next(1, 100)));
        return (a, b);
    }

    static void PrintBanner()
    {
        Console.WriteLine("MATRIX PERFORMANCE TEST SUITE v3.0");
    }
}

internal static class UowExtensions
{
    public static async Task<IEnumerable<T>> FindAsync<T>(
        this IUnitOfWork uow) where T : class
    {
        if (uow is UnitOfWork concreteUow)
        {
            var field = typeof(UnitOfWork).GetField(
                "context",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance);
            if (field?.GetValue(concreteUow) is ApplicationDbContext ctx)
                return await ctx.Set<T>().ToListAsync().ConfigureAwait(false);
        }
        return Enumerable.Empty<T>();
    }
}
