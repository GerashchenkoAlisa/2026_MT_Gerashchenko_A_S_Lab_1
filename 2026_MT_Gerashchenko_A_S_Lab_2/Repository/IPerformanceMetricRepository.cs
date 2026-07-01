using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public interface IPerformanceMetricRepository : IDataRepository<PerformanceMetric>
{
    Task<IEnumerable<PerformanceMetric>> GetByBenchmarkTestIdAsync(int benchmarkTestId);

    Task<IEnumerable<PerformanceMetric>> GetByServerConfigurationIdAsync(int serverConfigurationId);
}