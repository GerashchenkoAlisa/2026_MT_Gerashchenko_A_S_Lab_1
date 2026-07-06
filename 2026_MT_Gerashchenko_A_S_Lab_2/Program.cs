using Data;
using Entities;
using Factories;
using UnitsOfWork;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Data.Data;

namespace Program;

public static class Program
{
    public static async Task Main()
    {
        var services = new ServiceCollection();
        services.AddDbContext<BuildSystemDbContext>(options =>
            options.UseSqlite(DatabaseConfig.ConnectionString));
        services.AddScoped<IBuildSystemUnitOfWork, BuildSystemUnitOfWork>();
        services.AddScoped<ISystemDataFactory, DefaultSystemDataFactory>();

        var serviceProvider = services.BuildServiceProvider();
        try
        {
            var scope = serviceProvider.CreateAsyncScope();
            try
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BuildSystemDbContext>();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IBuildSystemUnitOfWork>();
                var factory = scope.ServiceProvider.GetRequiredService<ISystemDataFactory>();

                await dbContext.Database.MigrateAsync().ConfigureAwait(false);
                Banner(DatabaseConfig.MigrationCompleted);

                await SeedReferenceDataAsync(unitOfWork, factory).ConfigureAwait(false);

                var server = await SeedServerAsync(unitOfWork, factory).ConfigureAwait(false);
                await SeedBenchmarkTestsAsync(unitOfWork, factory).ConfigureAwait(false);
                var application = await SeedApplicationAsync(unitOfWork).ConfigureAwait(false);
                await DemonstrateBuildWorkflowAsync(unitOfWork, application).ConfigureAwait(false);
                await DemonstratePerformanceMetricsAsync(unitOfWork, server).ConfigureAwait(false);
                await GenerateSystemReportAsync(unitOfWork).ConfigureAwait(false);
            }
            finally
            {
                await scope.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            await serviceProvider.DisposeAsync().ConfigureAwait(false);
        }
    }

    private static async Task SeedReferenceDataAsync(IBuildSystemUnitOfWork uow, ISystemDataFactory factory)
    {
        Banner("Loading reference data into system");

        foreach (var stage in factory.CreateProcessStages())
        {
            var existing = await uow.ProcessStages.GetByIdAsync(stage.ProcessStageId).ConfigureAwait(false);
            if (existing is null)
            {
                await uow.ProcessStages.AddAsync(stage).ConfigureAwait(false);
            }
        }

        foreach (var severity in factory.CreateMessageSeverities())
        {
            var existing = await uow.MessageSeverities.GetByIdAsync(severity.MessageSeverityId).ConfigureAwait(false);
            if (existing is null)
            {
                await uow.MessageSeverities.AddAsync(severity).ConfigureAwait(false);
            }
        }

        foreach (var result in factory.CreateExecutionResults())
        {
            var existing = await uow.ExecutionResults.GetByIdAsync(result.ExecutionResultId).ConfigureAwait(false);
            if (existing is null)
            {
                await uow.ExecutionResults.AddAsync(result).ConfigureAwait(false);
            }
        }

        foreach (var env in factory.CreateSystemEnvironments())
        {
            var existing = await uow.SystemEnvironments.GetByIdAsync(env.SystemEnvironmentId).ConfigureAwait(false);
            if (existing is null)
            {
                await uow.SystemEnvironments.AddAsync(env).ConfigureAwait(false);
            }
        }

        foreach (var proc in factory.CreateProcessorModels())
        {
            var existing = await uow.ProcessorModels.GetByIdAsync(proc.ProcessorModelId).ConfigureAwait(false);
            if (existing is null)
            {
                await uow.ProcessorModels.AddAsync(proc).ConfigureAwait(false);
            }
        }

        foreach (var code in factory.CreateErrorCodes())
        {
            var existing = await uow.ErrorCodes.GetByCodeValueAsync(code.CodeValue).ConfigureAwait(false);
            if (existing is null)
            {
                await uow.ErrorCodes.AddAsync(code).ConfigureAwait(false);
            }
        }

        await uow.SaveChangesAsync().ConfigureAwait(false);
        Banner("Reference data seeding completed");
    }

    private static async Task<Entities.ServerConfiguration> SeedServerAsync(IBuildSystemUnitOfWork uow, ISystemDataFactory factory)
    {
        var existing = await uow.ServerConfigurations.GetDefaultServerAsync().ConfigureAwait(false);
        if (existing is not null)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Server already configured: ID={0}, Environment={1}, RAM={2}GB", existing.Id, existing.SystemEnvironmentId, existing.MemoryCapacityGb));
            return existing;
        }

        var server = factory.CreateServerConfiguration();
        await uow.ServerConfigurations.AddAsync(server).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);

        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Server configuration created: ID={0}, Processor={1}, RAM={2}GB, Environment={3}", server.Id, server.ProcessorModelId, server.MemoryCapacityGb, server.SystemEnvironmentId));
        return server;
    }

    private static async Task SeedBenchmarkTestsAsync(IBuildSystemUnitOfWork uow, ISystemDataFactory factory)
    {
        foreach (var test in factory.CreateBenchmarkTests())
        {
            var existing = await uow.BenchmarkTests
                .GetByTestDescriptionAsync(test.TestDescription).ConfigureAwait(false);
            if (existing is not null)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Benchmark test already exists: {0}", test.TestDescription));
                continue;
            }

            await uow.BenchmarkTests.AddAsync(test).ConfigureAwait(false);
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Benchmark test added: {0}", test.TestDescription));
        }

        await uow.SaveChangesAsync().ConfigureAwait(false);
    }

    private static async Task<Entities.Application> SeedApplicationAsync(IBuildSystemUnitOfWork uow)
    {
        var existing = await uow.Applications
            .GetByRepositoryPathAsync("/repositories/coreapp")
            .ConfigureAwait(false);
        if (existing is not null)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Application already registered: ID={0}, Name={1}", existing.Id, existing.ApplicationName));
            return existing;
        }

        var application = new Entities.Application
        {
            ApplicationName = "CoreApplication",
            RepositoryPath = "/repositories/coreapp",
        };
        await uow.Applications.AddAsync(application).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);

        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Application created: ID={0}, Name={1}", application.Id, application.ApplicationName));
        return application;
    }

    private static async Task DemonstrateBuildWorkflowAsync(
        IBuildSystemUnitOfWork uow, Entities.Application application)
    {
        Banner("Build Workflow Demonstration");

        var compileStage = await uow.ProcessStages.GetByIdAsync(1).ConfigureAwait(false);
        if (compileStage is null)
        {
            Banner("Compilation stage not found in database");
            return;
        }

        var existingExecutions = await uow.BuildExecutions
            .GetByApplicationIdAsync(application.Id).ConfigureAwait(false);
        if (existingExecutions.Any(e => e.ProcessStageId == compileStage.Id))
        {
            Banner("Execution already exists for this application");
            return;
        }

        var errorCode1 = await uow.ErrorCodes.GetByCodeValueAsync("ERR001").ConfigureAwait(false)
                         ?? new ErrorCode { CodeValue = "ERR001", CodeDescription = "Type resolution failed" };
        var errorCode2 = await uow.ErrorCodes.GetByCodeValueAsync("ERR002").ConfigureAwait(false)
                         ?? new ErrorCode { CodeValue = "ERR002", CodeDescription = "Undefined symbol reference" };

        await uow.BeginTransactionAsync().ConfigureAwait(false);
        try
        {
            var execution = new BuildExecution
            {
                ApplicationId = application.Id,
                ProcessStageId = compileStage.Id,
                ExecutionResultId = 2,
                ExecutionStartTime = DateTime.UtcNow,
                ExecutionTimeMs = 3_742,
                ExitCode = 1,
            };
            await uow.BuildExecutions.AddAsync(execution).ConfigureAwait(false);
            await uow.SaveChangesAsync().ConfigureAwait(false);

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Build execution created: ID={0}, Stage={1}, Result={2}, Duration={3}ms", execution.Id, compileStage.StageName, execution.ExecutionResultId, execution.ExecutionTimeMs));

            var messages = new[]
            {
                new BuildMessage
                {
                    BuildExecutionId = execution.Id,
                    MessageTimestamp = DateTime.UtcNow,
                    MessageSeverityId = 1,
                    ErrorCode = errorCode1,
                    MessageText = "Failed to resolve type 'CustomNamespace.CustomType'",
                },
                new BuildMessage
                {
                    BuildExecutionId = execution.Id,
                    MessageTimestamp = DateTime.UtcNow.AddMilliseconds(10),
                    MessageSeverityId = 1,
                    ErrorCode = errorCode2,
                    MessageText = "Reference to undefined symbol 'methodName'",
                },
            };

            foreach (var message in messages)
            {
                await uow.BuildMessages.AddAsync(message).ConfigureAwait(false);
            }

            await uow.SaveChangesAsync().ConfigureAwait(false);
            await uow.CommitTransactionAsync().ConfigureAwait(false);

            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Build messages recorded: {0} messages", messages.Length));

            var executionWithMessages = await uow.BuildExecutions
                .GetWithMessagesAsync(execution.Id).ConfigureAwait(false);

            if (executionWithMessages is not null)
            {
                Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Build execution retrieved: ID={0}, Messages={1}", executionWithMessages.Id, executionWithMessages.BuildMessages.Count));

                foreach (var msg in executionWithMessages.BuildMessages)
                {
                    Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Message: [{0}] {1}: {2}", msg.MessageSeverity?.SeverityName, msg.ErrorCode?.CodeValue, msg.MessageText));
                }
            }
        }
        catch (Exception ex)
        {
            await uow.RollbackTransactionAsync().ConfigureAwait(false);
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Transaction reverted: {0}", ex.Message));
            throw;
        }
    }

    private static async Task DemonstratePerformanceMetricsAsync(
        IBuildSystemUnitOfWork uow, Entities.ServerConfiguration server)
    {
        Banner("Performance Benchmark Analysis");

        var allTests = await uow.BenchmarkTests.GetAllAsync().ConfigureAwait(false);
        var benchmark = allTests.FirstOrDefault();
        if (benchmark is null)
        {
            Banner("No benchmark test available");
            return;
        }

        var allExecutions = await uow.BuildExecutions.GetAllAsync().ConfigureAwait(false);
        var execution = allExecutions.FirstOrDefault();
        if (execution is null)
        {
            Banner("No build execution available");
            return;
        }

        var serverMetrics = await uow.PerformanceMetrics.GetByServerConfigurationIdAsync(server.Id).ConfigureAwait(false);
        if (serverMetrics.Any(m => m.BenchmarkTestId == benchmark.Id))
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Performance metric already recorded: {0}", benchmark.TestDescription));
            return;
        }

        const long singleMs = 8_240;
        const long multiMs = 1_340;
        var gain = Math.Round((decimal)singleMs / multiMs, 4);

        var metric = new Entities.PerformanceMetric
        {
            BenchmarkTestId = benchmark.Id,
            ServerConfigurationId = server.Id,
            BuildExecutionId = execution.Id,
            SingleThreadTimeMs = singleMs,
            MultiThreadTimeMs = multiMs,
            MetricRecordTime = DateTime.UtcNow,
        };

        await uow.PerformanceMetrics.AddAsync(metric).ConfigureAwait(false);
        await uow.SaveChangesAsync().ConfigureAwait(false);

        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "Performance metric stored: Benchmark={0}, SingleThread={1}ms, MultiThread={2}ms, Gain={3}x, ID={4}", benchmark.TestDescription, singleMs, multiMs, gain, metric.Id));
    }

    private static async Task GenerateSystemReportAsync(IBuildSystemUnitOfWork uow)
    {
        Banner("===== SYSTEM REPORT =====");

        var applications = await uow.Applications.GetAllAsync().ConfigureAwait(false);
        Banner("--- Registered Applications ---");
        foreach (var app in applications)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", app.Id, app.ApplicationName));
        }

        var executions = await uow.BuildExecutions.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "--- Build Executions ({0}) ---", executions.Count()));
        foreach (var exe in executions)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                "[{0}] Result={1}, Errors={2}, Warnings={3}, Duration={4}ms",
                exe.Id,
                exe.ExecutionResult?.ResultName ?? exe.ExecutionResultId.ToString(CultureInfo.InvariantCulture),
                exe.ErrorCount,
                exe.WarningCount,
                exe.ExecutionTimeMs));
        }

        var messages = await uow.BuildMessages.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "--- Build Messages ({0}) ---", messages.Count()));
        foreach (var msg in messages)
        {
            Console.WriteLine(string.Format(
                CultureInfo.InvariantCulture,
                "[{0}] {1}: {2}",
                msg.MessageSeverity?.SeverityName,
                msg.ErrorCode?.CodeValue,
                msg.MessageText));
        }

        var metrics = await uow.PerformanceMetrics.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "--- Performance Metrics ({0}) ---", metrics.Count()));
        foreach (var m in metrics)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}] SingleThread={1}ms, MultiThread={2}ms, Gain={3}x", m.Id, m.SingleThreadTimeMs, m.MultiThreadTimeMs, m.PerformanceGain));
        }

        var stages = await uow.ProcessStages.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "--- Build Stages ({0}) ---", stages.Count()));
        foreach (var stg in stages)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", stg.Id, stg.StageName));
        }

        var processors = await uow.ProcessorModels.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "--- Processor Models ({0}) ---", processors.Count()));
        foreach (var proc in processors)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}] {1}, Cores={2}, Threads={3}", proc.Id, proc.ProcessorName, proc.PhysicalCores, proc.LogicalCores));
        }

        var servers = await uow.ServerConfigurations.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "--- Server Configurations ({0}) ---", servers.Count()));
        foreach (var srv in servers)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}] Processor={1}, RAM={2}GB, Environment={3}", srv.Id, srv.ProcessorModelId, srv.MemoryCapacityGb, srv.SystemEnvironment));
        }

        var benchmarks = await uow.BenchmarkTests.GetAllAsync().ConfigureAwait(false);
        Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "--- Benchmark Tests ({0}) ---", benchmarks.Count()));
        foreach (var bench in benchmarks)
        {
            Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "[{0}] {1}", bench.Id, bench.TestDescription));
        }
    }

    private static void Banner(string text)
    {
        Console.WriteLine();
        Console.WriteLine(text);
    }
}

