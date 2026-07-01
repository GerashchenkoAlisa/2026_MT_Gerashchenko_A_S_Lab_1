namespace _2026_MT_Gerashchenko_A_S_Lab_4.Models;
public record DownloadResult
{
    public DownloadResult(Uri url, bool isSuccess, long bytesWritten, string? error = null)
    {
        Url = url;
        IsSuccess = isSuccess;
        BytesWritten = bytesWritten;
        Error = error;
    }

    public Uri Url { get; }
    public bool IsSuccess { get; }
    public long BytesWritten { get; }
    public string? Error { get; }
}