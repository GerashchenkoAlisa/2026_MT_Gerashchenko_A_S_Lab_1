namespace _2026_MT_Gerashchenko_A_S_Lab_5.Models;

public record TopMethodEntry(
    string test,
    long timeMs,
    decimal gain,
    string processor,
    string algorithm,
    bool isParallel);

public record SpeedupEntry(
    string testType,
    double avgSpeedup,
    int count);

public record AnomalyEntry(
    string test,
    long singleMs,
    long multiMs,
    long overhead);

public record EnvironmentEntry(
    string processor,
    double avgSingle,
    double avgMulti,
    int count);

public record BestStructureEntry(
    string sizeGroup,
    decimal bestGain);

public record OrderComparisonEntry(
    string test,
    double avgTime);