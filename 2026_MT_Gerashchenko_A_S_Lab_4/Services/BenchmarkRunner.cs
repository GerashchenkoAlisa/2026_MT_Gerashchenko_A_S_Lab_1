using System.Diagnostics;
using Models;
using Services;

namespace Services;
public sealed class BenchmarkRunner(IHttpProcessor processor, IConsoleWriter console)
{
    private static readonly int[] ParallelismLevels = [1, 10, 50];

    public async Task<IReadOnlyList<ScanRunResult>> RunScanBenchmarksAsync(
        IReadOnlyList<Uri> urls,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(urls);

        var results = new List<ScanRunResult>();

        foreach (var parallelism in ParallelismLevels)
        {
            await console.WriteLineAsync($"[Scan] maxParallelism={parallelism} - starting...", ct).ConfigureAwait(false);

            var stopwatch = Stopwatch.StartNew();
            var scanResults = (await processor.AnalyzeAsync(urls, parallelism, ct).ConfigureAwait(false)).ToList();
            stopwatch.Stop();

            var run = new ScanRunResult(
                OperationType.Scan,
                parallelism,
                stopwatch.Elapsed,
                scanResults.Count(r => r.IsSuccess),
                scanResults.Count(r => !r.IsSuccess));

            await console.WriteLineAsync(
                $"[Scan] maxParallelism={parallelism} - done in {stopwatch.Elapsed.TotalSeconds:F2}s | success={run.SuccessCount} errors={run.ErrorCount}",
                ct).ConfigureAwait(false);

            results.Add(run);
        }

        return results;
    }

    public async Task<IReadOnlyList<ScanRunResult>> RunDownloadBenchmarksAsync(
        IReadOnlyList<Uri> urls,
        string destinationDir,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(urls);
        ArgumentNullException.ThrowIfNull(destinationDir);

        var results = new List<ScanRunResult>();

        foreach (var parallelism in ParallelismLevels)
        {
            await console.WriteLineAsync($"[Download] maxParallelism={parallelism} - starting...", ct).ConfigureAwait(false);

            var stopwatch = Stopwatch.StartNew();
            var downloadResults = (await processor
                .DownloadAsync(urls, Path.Combine(destinationDir, $"p{parallelism}"), parallelism, ct)
                .ConfigureAwait(false)).ToList();
            stopwatch.Stop();

            foreach (var dr in downloadResults.Where(r => !r.IsSuccess && r.Error is not null))
            {
                await console.WriteErrorLineAsync($"[Download error] {dr.Url}: {dr.Error}", ct).ConfigureAwait(false);
            }

            var run = new ScanRunResult(
                OperationType.Download,
                parallelism,
                stopwatch.Elapsed,
                downloadResults.Count(r => r.IsSuccess),
                downloadResults.Count(r => !r.IsSuccess));

            await console.WriteLineAsync(
                $"[Download] maxParallelism={parallelism} - done in {stopwatch.Elapsed.TotalSeconds:F2}s | success={run.SuccessCount} errors={run.ErrorCount}",
                ct).ConfigureAwait(false);

            results.Add(run);
        }

        return results;
    }
}