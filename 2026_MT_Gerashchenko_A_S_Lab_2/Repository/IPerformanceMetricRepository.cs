using Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repository;

public interface IPerformanceMetricRepository : IDataRepository<PerformanceMetric>
{
    Task<IEnumerable<PerformanceMetric>> GetByBenchmarkTestIdAsync(int benchmarkTestId);

    Task<IEnumerable<PerformanceMetric>> GetByServerConfigurationIdAsync(int serverConfigurationId);
}