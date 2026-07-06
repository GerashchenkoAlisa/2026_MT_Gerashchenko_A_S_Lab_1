using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class PerformanceMetricRepository(BuildSystemDbContext context)
    : BaseDataRepository<PerformanceMetric>(context), IPerformanceMetricRepository
{
    public async Task<IEnumerable<PerformanceMetric>> GetByBenchmarkTestIdAsync(int benchmarkTestId)
    {
        return await this.DbSet
            .Include(pm => pm.ServerConfiguration)
            .ThenInclude(sc => sc!.ProcessorModel)
            .Include(pm => pm.BenchmarkTest)
            .Where(pm => pm.BenchmarkTestId == benchmarkTestId)
            .OrderByDescending(pm => pm.MetricRecordTime)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<PerformanceMetric>> GetByServerConfigurationIdAsync(int serverConfigurationId)
    {
        return await this.DbSet
            .Include(pm => pm.BenchmarkTest)
            .Include(pm => pm.ServerConfiguration)
            .ThenInclude(sc => sc!.ProcessorModel)
            .Where(pm => pm.ServerConfigurationId == serverConfigurationId)
            .OrderByDescending(pm => pm.MetricRecordTime)
            .ToListAsync().ConfigureAwait(false);
    }
}