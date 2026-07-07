// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using Data;
using Entities;
using MatrixLib;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MTLAB3.MatrixLib;
using UnitsOfWork;

namespace MTLAB3.PerformanceTest;

internal sealed record BenchResult(
    int size,
    string testType,
    string algorithm,
    string storage,
    long microseconds);

internal static class Program
{
    private static readonly List<BenchResult> Results =
        [];

    private static string cpuModelName = "Unknown CPU";
    private static int physCores = Environment.ProcessorCount / 2;
    private static int logCores = Environment.ProcessorCount;
    private static decimal ramGb = 0m;
    private static string osName = Environment.OSVersion.ToString();

    private static string ResolveDbPath()
    {
        string binDir = AppContext.BaseDirectory;
        string projDir = Path.GetFullPath(Path.Combine(binDir, "..", "..", ".."));
        string slnDir = Path.GetFullPath(Path.Combine(projDir, ".."));

        string[] candidates =
        [
            Path.Combine(slnDir, "2026_MT_Gerashchenko_A_S_Lab_2", "app.db"),
            Path.Combine(slnDir, "..", "2026_MT_Gerashchenko_A_S_Lab_2", "app.db"),
        ];

        foreach (string c in candidates)
        {
            string full = Path.GetFullPath(c);
            if (File.Exists(full))
            {
                return full;
            }
        }

        return Path.GetFullPath(candidates[0]);
    }

    private static async Task Main()
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
        await SaveResultsToDatabaseAsync().ConfigureAwait(false);

        Console.ReadKey();
    }

    private static void CollectSystemInfo()
    {
        cpuModelName = Environment.GetEnvironmentVariable("processor_identifier")
                        ?? "Unknown CPU";
    }

    private static void PrintSystemInfo()
    {
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"   CPU  : {cpuModelName}");
        Console.WriteLine($"   Cores: {physCores} physical / {logCores} logical");
        Console.WriteLine($"   RAM  : {(ramGb > 0 ? $"{ramGb} GB" : "unknown")}");
        Console.WriteLine($"   OS   : {osName}");
    }

    private static void RunBenchmarkForSize(int size)
    {
        var rng = new Random(1);
        var (rA, rB) = MakePair<int>(size, (r, c) => new RectMatrix<int>(r, c));
        var (jA, jB) = MakePair<int>(size, (r, c) => new JaggedMatrix<int>(r, c));
        var (fA, fB) = MakePair<int>(size, (r, c) => new FlatMatrix<int>(r, c));

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
            {
                BenchmarkMultiplication(a, b, name, size);
            }
            else
            {
                Console.WriteLine($"   {name}: skipping multiplication for {size}x{size}");
            }
        }
    }

    private static void BenchmarkAddition(IMatrix<int> a, IMatrix<int> b, string storage, int size)
    {
        Measure("Addition", "AddByRowsSequential", storage, size, () => a.AddByRowsSequential(b));
        Measure("Addition", "AddByRowsParallel", storage, size, () => a.AddByRowsParallel(b));
        Measure("Addition", "AddByColumnsSequential", storage, size, () => a.AddByColumnsSequential(b));
        Measure("Addition", "AddByColumnsParallel", storage, size, () => a.AddByColumnsParallel(b));
    }

    private static void BenchmarkMultiplication(IMatrix<int> a, IMatrix<int> b, string storage, int size)
    {
        Measure("Multiply", "MultiplySequential", storage, size, () => a.MultiplySequential(b));
        Measure("Multiply", "MultiplyParallel", storage, size, () => a.MultiplyParallel(b));
        Measure("Multiply", "MultiplyOptimSequential", storage, size, () => a.MultiplyOptimSequential(b));
        Measure("Multiply", "MultiplyOptimParallel", storage, size, () => a.MultiplyOptimParallel(b));
        Measure("Multiply", "MultiplyNaiveSequential", storage, size, () => a.MultiplyNaiveSequential(b));
        Measure("Multiply", "MultiplyNaiveParallel", storage, size, () => a.MultiplyNaiveParallel(b));
    }

    private static void Measure(string testType, string algorithm, string storage, int size, Action action)
    {
        const int Warmup = 1;
        const int Runs = 3;

        for (int w = 0; w < Warmup; w++)
        {
            action();
        }

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

    private static void FindBreakevenPoint()
    {
        Console.WriteLine(new string('-', 65));

        int[] sizes =
            [10, 50, 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000];
        const int Runs = 5;
        const int ConsecutiveReq = 3;

        int wins = 0;
        int? breakevenN = null;
        var rng = new Random(7);

        foreach (int size in sizes)
        {
            var (a, b) = MakePair<int>(size, (r, c) => new RectMatrix<int>(r, c));

            long seqUs = MedianMicros(Runs, () => a.AddByRowsSequential(b));
            long parUs = MedianMicros(Runs, () => a.AddByRowsParallel(b));
            double ratio = (double)seqUs / Math.Max(1, parUs);

            string mark = parUs < seqUs ? "parallel faster" : "sequential faster";
            Console.WriteLine($"   {size,5}x{size,-5} | seq={seqUs,7} us | par={parUs,7} us | ratio={ratio:F2} | {mark}");

            if (parUs < seqUs)
            {
                wins++;
                if (wins >= ConsecutiveReq && breakevenN is null)
                {
                    breakevenN = size;
                    Console.WriteLine($"\n stable breakeven at {size}x{size} ({ConsecutiveReq} consecutive parallel wins)\n");
                }
            }
            else
            {
                wins = 0;
            }
        }

        if (breakevenN is null)
        {
        }
    }

    private static long MedianMicros(int runs, Action action)
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

    private static void PrintSummary()
    {
        Console.WriteLine(new string('-', 72));

        foreach (var g in Results.GroupBy(r => new { r.size, r.testType, r.algorithm })
            .OrderBy(g => g.Key.Size)
            .ThenBy(g => g.Key.TestType)
            .ThenBy(g => g.Key.Algorithm))
        {
            double avg = g.Average(r => r.Microseconds);
            Console.WriteLine($"   {g.Key.Size,5}x{g.Key.Size} | {g.Key.Algorithm,-25} | {avg,8:F0} us");
        }
    }

    private static async Task SaveResultsToDatabaseAsync()
    {
        string dbPath = ResolveDbPath();
        string tmpDb = Path.Combine(Path.GetTempPath(), $"lab3_{Guid.NewGuid():N}.db");
        Console.WriteLine($"\nSaving results to database: {dbPath}");

        File.Copy(dbPath, tmpDb, overwrite: true);

        var services = new ServiceCollection();
        services.AddDbContext<BuildSystemDbContext>(opts => opts.UseSqlite($"Data Source={tmpDb}"));
        services.AddScoped<IBuildSystemUnitOfWork, BuildSystemUnitOfWork>();

        bool success = false;
        await using var sp = services.BuildServiceProvider();
        await using var scope = sp.CreateAsyncScope();
        var uow = scope.ServiceProvider.GetRequiredService<IBuildSystemUnitOfWork>();

        try
        {
            var processor = (await uow.ProcessorModels.FindAsync(p => p.ProcessorName.Contains(cpuModelName)).ConfigureAwait(false))
                            .FirstOrDefault() ?? new ProcessorModel
                            {
                                ProcessorName = cpuModelName,
                                PhysicalCores = physCores,
                                LogicalCores = logCores,
                            };

            if (processor.ProcessorModelId == 0)
            {
                await uow.ProcessorModels.AddAsync(processor).ConfigureAwait(false);
                await uow.SaveChangesAsync().ConfigureAwait(false);
            }

            var env = (await uow.SystemEnvironments.FindAsync(e => e.EnvironmentName == osName).ConfigureAwait(false))
                      .FirstOrDefault() ?? new SystemEnvironment { EnvironmentName = osName };

            if (env.SystemEnvironmentId == 0)
            {
                await uow.SystemEnvironments.AddAsync(env).ConfigureAwait(false);
                await uow.SaveChangesAsync().ConfigureAwait(false);
            }

            var benchmark = new BenchmarkTest { TestDescription = $"Matrix Operations {DateTime.UtcNow:yyyy-MM-dd}" };
            await uow.BenchmarkTests.AddAsync(benchmark).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            foreach (var r in Results)
            {
                var metric = new PerformanceMetric
                {
                    BenchmarkTestId = benchmark.BenchmarkTestId,
                    ServerConfigurationId = 1,
                    BuildExecutionId = 1,
                    SingleThreadTimeMs = r.microseconds / 1000,
                    MultiThreadTimeMs = r.microseconds / 1000,
                    MetricRecordTime = DateTime.UtcNow,
                };
                await uow.PerformanceMetrics.AddAsync(metric).ConfigureAwait(false);
            }

            await uow.SaveChangesAsync().ConfigureAwait(false);
            success = true;
            Console.WriteLine($"Saved {Results.Count} performance records to DB.");
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine($"DB save failed: {ex.Message}");
        }
        finally
        {
            if (success)
            {
                File.Copy(tmpDb, dbPath, overwrite: true);
            }

            if (File.Exists(tmpDb))
            {
                File.Delete(tmpDb);
            }
        }
    }

    private static (IMatrix<T> a, IMatrix<T> b) MakePair<T>(int size, Func<int, int, IMatrix<T>> factory)
        where T : INumber<T>
    {
        var a = factory(size, size);
        var b = factory(size, size);

        FillMatrixWithSecureRandom(a);
        FillMatrixWithSecureRandom(b);

        return (a, b);

        static void FillMatrixWithSecureRandom(IMatrix<T> matrix)
        {
            var buffer = new byte[sizeof(int)];
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    int value;
                    do
                    {
                        RandomNumberGenerator.Fill(buffer);
                        value = BitConverter.ToInt32(buffer, 0) & int.MaxValue;
                        value = 1 + (value % 99);
                    }
                    while (value < 1 || value > 99);

                    matrix.Fill((row, col) => row == i && col == j ? T.CreateChecked(value) : matrix[row, col]);
                }
            }
        }
    }

    private static void PrintBanner()
    {
    }
}
