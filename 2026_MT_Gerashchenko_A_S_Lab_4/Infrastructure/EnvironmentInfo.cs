namespace _2026_MT_Gerashchenko_A_S_Lab_4.Infrastructure;
public sealed record EnvironmentInfo
{
    public EnvironmentInfo(
        string osDescription,
        decimal ramGb,
        string cpuModel,
        int physicalCoreCount,
        int logicalThreadCount)
    {
        this.OsDescription = osDescription;
        this.RamGb = ramGb;
        this.CpuModel = cpuModel;
        this.PhysicalCoreCount = physicalCoreCount;
        this.LogicalThreadCount = logicalThreadCount;
    }

    public string OsDescription { get; }
    public decimal RamGb { get; }
    public string CpuModel { get; }
    public int PhysicalCoreCount { get; }
    public int LogicalThreadCount { get; }
}