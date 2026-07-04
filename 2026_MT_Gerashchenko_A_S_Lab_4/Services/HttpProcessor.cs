using System.Diagnostics;
using Infrastructure;
using Models;

namespace _2026_MT_Gerashchenko_A_S_Lab_4.Services;
public sealed class HttpProcessor(IHttpClientFactory httpClientFactory) : IHttpProcessor
{
    private const long MaxDownloadBytes = 10 * 1024 * 1024;

    public async Task<IEnumerable<ScanResult>> AnalyzeAsync(
        IEnumerable<Uri> urls,
        int maxParallelism,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(urls);

        var urlList = urls.ToList();
        var results = new ScanResult?[urlList.Count];
        using var semaphore = new SemaphoreSlim(maxParallelism);

        var tasks = urlList.Select(async (url, index) =>
        {
            await semaphore.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                results[index] = await ScanUrlAsync(url, ct).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks).ConfigureAwait(false);

        return results.Where(r => r is not null).Select(r => r!);
    }

    public async Task<IEnumerable<DownloadResult>> DownloadAsync(
        IEnumerable<Uri> urls,
        string destinationDir,
        int maxParallelism,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(urls);
        ArgumentNullException.ThrowIfNull(destinationDir);

        Directory.CreateDirectory(destinationDir);

        var urlList = urls.ToList();
        var results = new DownloadResult[urlList.Count];

        await Parallel.ForEachAsync(
            urlList.Select((url, i) => (url, i)),
            new ParallelOptions { MaxDegreeOfParallelism = maxParallelism, CancellationToken = ct },
            async (item, token) =>
            {
                results[item.i] = await this.DownloadUrlAsync(item.url, destinationDir, token).ConfigureAwait(false);
            }).ConfigureAwait(false);

        return results;
    }

    private static async Task<ScanResult> ScanUrlAsync(Uri url, CancellationToken ct)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };
        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var response = await client
                .GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct)
                .ConfigureAwait(false);

            stopwatch.Stop();
            var contentLength = response.Content.Headers.ContentLength ?? -1;

            return new ScanResult(url, (int)response.StatusCode, contentLength, stopwatch.Elapsed);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or TimeoutException)
        {
            stopwatch.Stop();
            return new ScanResult(url, null, -1, stopwatch.Elapsed, ex.Message);
        }
    }

    private async Task<DownloadResult> DownloadUrlAsync(Uri url, string destinationDir, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient();
        var filePath = Path.Combine(destinationDir, FileNameSanitizer.Sanitize(url));
        var completed = false;

        try
        {
            using var response = await client
                .GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct)
                .ConfigureAwait(false);

            response.EnsureSuccessStatusCode();

            var contentLength = response.Content.Headers.ContentLength;
            if (contentLength.HasValue && contentLength.Value > MaxDownloadBytes)
            {
                return new DownloadResult(url, false, 0, $"File too large: {contentLength.Value} bytes");
            }

            var contentStream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);

            await using (contentStream.ConfigureAwait(false))
            {
                var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, true);
                await using (fileStream.ConfigureAwait(false))
                {
                    var buffer = new byte[81920];
                    long totalRead = 0;
                    int bytesRead;

                    while ((bytesRead = await contentStream.ReadAsync(buffer, ct).ConfigureAwait(false)) > 0)
                    {
                        totalRead += bytesRead;
                        if (totalRead > MaxDownloadBytes)
                        {
                            return new DownloadResult(url, false, totalRead, "Exceeded size limit mid-stream");
                        }

                        await fileStream.WriteAsync(buffer.AsMemory(0, bytesRead), ct).ConfigureAwait(false);
                    }

                    completed = true;
                    return new DownloadResult(url, true, totalRead);
                }
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or TimeoutException)
        {
            return new DownloadResult(url, false, 0, ex.Message);
        }
        finally
        {
            if (!completed && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}