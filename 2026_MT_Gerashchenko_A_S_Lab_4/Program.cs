using _2026_MT_Gerashchenko_A_S_Lab_4.Infrastructure;
using _2026_MT_Gerashchenko_A_S_Lab_4.Persistence;
using _2026_MT_Gerashchenko_A_S_Lab_4.Services;
using Microsoft.Extensions.DependencyInjection;
using Services;

namespace Lab4;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddLab4Services("lab4.db");

        var serviceProvider = services.BuildServiceProvider();

        await using (serviceProvider.ConfigureAwait(false))
        {
            using var cts = new CancellationTokenSource();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            var urlsFile = args.Length > 0 ? args[0] : ResolveDefaultPath("urls.txt");
            var downloadsDir = args.Length > 1 ? args[1] : "downloads";

            var console = serviceProvider.GetRequiredService<IConsoleWriter>();

            IReadOnlyList<Uri> urls;
            try
            {
                urls = await UrlListProvider.LoadAsync(urlsFile, cts.Token).ConfigureAwait(false);
                await console.WriteLineAsync($"Loaded {urls.Count} URLs from {urlsFile}", cts.Token).ConfigureAwait(false);
            }
            catch (FileNotFoundException ex)
            {
                await console.WriteErrorLineAsync(ex.Message, cts.Token).ConfigureAwait(false);
                return 1;
            }

            var env = EnvironmentInfoProvider.Collect();
            await console.WriteLineAsync(
                $"Environment: OS={env.OsDescription}, CPU={env.CpuModel}, Cores={env.PhysicalCoreCount}, Threads={env.LogicalThreadCount}, RAM={env.RamGb}GB",
                cts.Token).ConfigureAwait(false);

            var scope = serviceProvider.CreateAsyncScope();
            await using (scope.ConfigureAwait(false))
            {
                var benchmarkRunner = scope.ServiceProvider.GetRequiredService<BenchmarkRunner>();
                var persistenceService = scope.ServiceProvider.GetRequiredService<MetricsPersistenceService>();

                await console.WriteLineAsync(string.Empty, cts.Token).ConfigureAwait(false);
                await console.WriteLineAsync("=== SCAN BENCHMARKS ===", cts.Token).ConfigureAwait(false);
                var scanRuns = await benchmarkRunner.RunScanBenchmarksAsync(urls, cts.Token).ConfigureAwait(false);

                await console.WriteLineAsync(string.Empty, cts.Token).ConfigureAwait(false);
                await console.WriteLineAsync("=== DOWNLOAD BENCHMARKS ===", cts.Token).ConfigureAwait(false);
                var downloadRuns = await benchmarkRunner
                    .RunDownloadBenchmarksAsync(urls, downloadsDir, cts.Token)
                    .ConfigureAwait(false);

                await console.WriteLineAsync(string.Empty, cts.Token).ConfigureAwait(false);
                await console.WriteLineAsync("=== SAVING METRICS TO DATABASE ===", cts.Token).ConfigureAwait(false);
                var allRuns = scanRuns.Concat(downloadRuns).ToList();
                await persistenceService.SaveRunsAsync(allRuns, env, cts.Token).ConfigureAwait(false);
                await console.WriteLineAsync("Metrics saved.", cts.Token).ConfigureAwait(false);

                await console.WriteLineAsync(string.Empty, cts.Token).ConfigureAwait(false);
                await console.WriteLineAsync("=== SUMMARY ===", cts.Token).ConfigureAwait(false);
                foreach (var run in allRuns)
                {
                    await console.WriteLineAsync(
                        $"{run.OperationType,-10} | p={run.MaxParallelism,-3} | {run.TotalElapsed.TotalSeconds,8:F2}s | ok={run.SuccessCount} err={run.ErrorCount}",
                        cts.Token).ConfigureAwait(false);
                }
            }
        }

        return 0;
    }

    private static string ResolveDefaultPath(string fileName)
    {
        var nextToExe = Path.Combine(AppContext.BaseDirectory, fileName);
        return File.Exists(nextToExe) ? nextToExe : Path.Combine(Directory.GetCurrentDirectory(), fileName);
    }
}