using Models;

namespace Services;
public interface IHttpProcessor
{
    Task<IEnumerable<ScanResult>> AnalyzeAsync(
        IEnumerable<Uri> urls,
        int maxParallelism,
        CancellationToken ct);

    Task<IEnumerable<DownloadResult>> DownloadAsync(
        IEnumerable<Uri> urls,
        string destinationDir,
        int maxParallelism,
        CancellationToken ct);
}