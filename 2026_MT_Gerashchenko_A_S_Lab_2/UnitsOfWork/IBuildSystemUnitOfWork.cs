using Entities;
using Repository;
using System;
using System.Threading.Tasks;

namespace UnitsOfWork;

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

    IDataRepository<MessageSeverity> MessageSeverities { get; }

    IDataRepository<ExecutionResult> ExecutionResults { get; }

    Task<int> SaveChangesAsync();

    Task BeginTransactionAsync();

    Task CommitTransactionAsync();

    Task RollbackTransactionAsync();
}