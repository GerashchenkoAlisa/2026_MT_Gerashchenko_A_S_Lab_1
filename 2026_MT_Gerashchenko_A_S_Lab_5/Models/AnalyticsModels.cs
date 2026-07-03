namespace _2026_MT_Gerashchenko_A_S_Lab_5.Models;

public record TopMethodEntry(
    string Test,
    long TimeMs,
    decimal Gain,
    string Processor,
    string Algorithm,
    bool IsParallel
);

public record SpeedupEntry(
    string TestType,
    double AvgSpeedup,
    int Count
);

public record AnomalyEntry(
    string Test,
    long SingleMs,
    long MultiMs,
    long Overhead
);

public record EnvironmentEntry(
    string Processor,
    double AvgSingle,
    double AvgMulti,
    int Count
);

public record BestStructureEntry(
    string SizeGroup,
    decimal BestGain
);

public record OrderComparisonEntry(
    string Test,
    double AvgTime
);