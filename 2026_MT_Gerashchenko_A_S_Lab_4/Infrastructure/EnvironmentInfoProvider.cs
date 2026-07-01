using System.Runtime.InteropServices;

namespace _2026_MT_Gerashchenko_A_S_Lab_4.Infrastructure;
public static class EnvironmentInfoProvider
{
    public static EnvironmentInfo Collect()
    {
        var osDescription = RuntimeInformation.OSDescription;
        var ramGb = Math.Round(
            (decimal)GC.GetGCMemoryInfo().TotalAvailableMemoryBytes / (1024 * 1024 * 1024), 2);

        var cpuModel = Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER")
                       ?? RuntimeInformation.ProcessArchitecture.ToString();

        var logicalThreads = Environment.ProcessorCount;

        return new EnvironmentInfo(
            osDescription,
            ramGb,
            cpuModel,
            logicalThreads / 2,
            logicalThreads);
    }
}