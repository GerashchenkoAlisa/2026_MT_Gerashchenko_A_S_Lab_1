using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using _2026_MT_Gerashchenko_A_S_Lab_2.Repository;
using System;
using System.Threading.Tasks;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.UnitsOfWork;

public interface IBuildSystemUnitOfWork : IDisposable
{
    IApplicationRepository Applications { get; }

    IDataRepository<ProcessStage> ProcessStages { get; }

    IBuildExecutionRepository BuildExecutions { get; }

    IBuildMessageRepository BuildMessages { get; }

    IDataRepository<ProcessorModel> ProcessorModels { get; }

    IServerConfigurationRepository ServerConfigurations { get; }

    IBenchmarkTestRepository BenchmarkTests { get; }

    IPerformanceMetricRepository PerformanceMetrics { get; }

    IErrorCodeRepository ErrorCodes { get; }

    IDataRepository<SystemEnvironment> SystemEnvironments { get; }

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}