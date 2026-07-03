namespace _2026_MT_Gerashchenko_A_S_Lab_4.Models;
public record ScanRunResult
{
    public ScanRunResult(
        OperationType operationType,
        int maxParallelism,
        TimeSpan totalElapsed,
        int successCount,
        int errorCount)
    {
        this.OperationType = operationType;
        this.MaxParallelism = maxParallelism;
        this.TotalElapsed = totalElapsed;
        this.SuccessCount = successCount;
        this.ErrorCount = errorCount;
    }

    public OperationType OperationType { get; }
    public int MaxParallelism { get; }
    public TimeSpan TotalElapsed { get; }
    public int SuccessCount { get; }
    public int ErrorCount { get; }
}