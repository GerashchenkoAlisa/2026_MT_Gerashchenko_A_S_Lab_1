namespace Models;
public record ScanResult
{
    public ScanResult(Uri url, int? statusCode, long contentLength, TimeSpan responseTime, string? error = null)
    {
        this.Url = url;
        this.StatusCode = statusCode;
        this.ContentLength = contentLength;
        this.ResponseTime = responseTime;
        this.Error = error;
    }

    public Uri Url { get; }
    public int? StatusCode { get; }
    public long ContentLength { get; }
    public TimeSpan ResponseTime { get; }
    public string? Error { get; }

    public bool IsSuccess => this.Error is null && this.StatusCode is >= 200 and < 300;
}