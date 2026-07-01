using _2026_MT_Gerashchenko_A_S_Lab_4.Models;

namespace _2026_MT_Gerashchenko_A_S_Lab_4.Services;
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