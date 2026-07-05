// <copyright file="IPerformanceMetricRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Entities;

namespace Repository;

public interface IPerformanceMetricRepository : IDataRepository<PerformanceMetric>
{
    Task<IEnumerable<PerformanceMetric>> GetByBenchmarkTestIdAsync(int benchmarkTestId);

    Task<IEnumerable<PerformanceMetric>> GetByServerConfigurationIdAsync(int serverConfigurationId);
}