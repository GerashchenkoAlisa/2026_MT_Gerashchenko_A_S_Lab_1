using Data;
using Entities;
using Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Threading.Tasks;

namespace UnitsOfWork;

public class BuildSystemUnitOfWork(BuildSystemDbContext context)
    : IBuildSystemUnitOfWork
{
    private readonly BuildSystemDbContext context = context;
    private IDbContextTransaction? transaction;
    private bool disposed;

    private IApplicationRepository? applications;
    private IDataRepository<ProcessStage>? processStages;
    private IBuildExecutionRepository? buildExecutions;
    private IBuildMessageRepository? buildMessages;
    private IErrorCodeRepository? errorCodes;
    private IDataRepository<ProcessorModel>? processorModels;
    private IServerConfigurationRepository? serverConfigurations;
    private IBenchmarkTestRepository? benchmarkTests;
    private IPerformanceMetricRepository? performanceMetrics;
    private IDataRepository<SystemEnvironment>? systemEnvironments;
    private IDataRepository<MessageSeverity>? messageSeverities;
    private IDataRepository<ExecutionResult>? executionResults;

    public IApplicationRepository Applications =>
        this.applications ??= new ApplicationRepository(this.context);

    public IDataRepository<ProcessStage> ProcessStages =>
        this.processStages ??= new BaseDataRepository<ProcessStage>(this.context);

    public IBuildExecutionRepository BuildExecutions =>
        this.buildExecutions ??= new BuildExecutionRepository(this.context);

    public IBuildMessageRepository BuildMessages =>
        this.buildMessages ??= new BuildMessageRepository(this.context);

    public IErrorCodeRepository ErrorCodes =>
        this.errorCodes ??= new ErrorCodeRepository(this.context);

    public IDataRepository<ProcessorModel> ProcessorModels =>
        this.processorModels ??= new BaseDataRepository<ProcessorModel>(this.context);

    public IServerConfigurationRepository ServerConfigurations =>
        this.serverConfigurations ??= new ServerConfigurationRepository(this.context);

    public IBenchmarkTestRepository BenchmarkTests =>
        this.benchmarkTests ??= new BenchmarkTestRepository(this.context);

    public IPerformanceMetricRepository PerformanceMetrics =>
        this.performanceMetrics ??= new PerformanceMetricRepository(this.context);

    public IDataRepository<SystemEnvironment> SystemEnvironments =>
        this.systemEnvironments ??= new BaseDataRepository<SystemEnvironment>(this.context);

    public IDataRepository<MessageSeverity> MessageSeverities =>
    this.messageSeverities ??= new BaseDataRepository<MessageSeverity>(this.context);

    public IDataRepository<ExecutionResult> ExecutionResults =>
        this.executionResults ??= new BaseDataRepository<ExecutionResult>(this.context);

    public async Task<int> SaveChangesAsync()
    {
        return await this.context.SaveChangesAsync().ConfigureAwait(false);
    }

    public async Task BeginTransactionAsync()
    {
        this.transaction = await this.context.Database.BeginTransactionAsync().ConfigureAwait(false);
    }

    public async Task CommitTransactionAsync()
    {
        if (this.transaction != null)
        {
            await this.transaction.CommitAsync().ConfigureAwait(false);
            await this.transaction.DisposeAsync().ConfigureAwait(false);
            this.transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (this.transaction != null)
        {
            await this.transaction.RollbackAsync().ConfigureAwait(false);
            await this.transaction.DisposeAsync().ConfigureAwait(false);
            this.transaction = null;
        }
    }

    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed && disposing)
        {
            this.transaction?.Dispose();
            this.context.Dispose();
        }

        this.disposed = true;
    }
}