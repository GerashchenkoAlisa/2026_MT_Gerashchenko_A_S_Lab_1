namespace _2026_MT_Gerashchenko_A_S_Lab_4.Models;
public record ScanResult
{
    public ScanResult(Uri url, int? statusCode, long contentLength, TimeSpan responseTime, string? error = null)
    {
        Url = url;
        StatusCode = statusCode;
        ContentLength = contentLength;
        ResponseTime = responseTime;
        Error = error;
    }

    public Uri Url { get; }
    public int? StatusCode { get; }
    public long ContentLength { get; }
    public TimeSpan ResponseTime { get; }
    public string? Error { get; }

    public bool IsSuccess => Error is null && StatusCode is >= 200 and < 300;
}