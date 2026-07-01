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
        OperationType = operationType;
        MaxParallelism = maxParallelism;
        TotalElapsed = totalElapsed;
        SuccessCount = successCount;
        ErrorCount = errorCount;
    }

    public OperationType OperationType { get; }
    public int MaxParallelism { get; }
    public TimeSpan TotalElapsed { get; }
    public int SuccessCount { get; }
    public int ErrorCount { get; }
}