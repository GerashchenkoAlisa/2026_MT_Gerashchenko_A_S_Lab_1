using Entities;
using System.Collections.Generic;

namespace Factories;

public class DefaultSystemDataFactory : ISystemDataFactory
{
    public IEnumerable<SystemEnvironment> CreateSystemEnvironments() =>
    [
        new SystemEnvironment { SystemEnvironmentId = 1, EnvironmentName = "Windows 11 Pro (64-bit)" },
        new SystemEnvironment { SystemEnvironmentId = 2, EnvironmentName = "Windows 10 Pro (64-bit)" },
        new SystemEnvironment { SystemEnvironmentId = 3, EnvironmentName = "Ubuntu 24.04 LTS (64-bit)" },
        new SystemEnvironment { SystemEnvironmentId = 4, EnvironmentName = "macOS Sequoia 15" },
    ];

    public IEnumerable<ProcessorModel> CreateProcessorModels() =>
    [
        new ProcessorModel { ProcessorModelId = 1, ProcessorName = "AMD Ryzen 9 7950X", PhysicalCores = 16, LogicalCores = 32 },
        new ProcessorModel { ProcessorModelId = 2, ProcessorName = "Intel Core i9-13900K", PhysicalCores = 24, LogicalCores = 32 },
        new ProcessorModel { ProcessorModelId = 3, ProcessorName = "AMD Ryzen 5 5600X", PhysicalCores = 6, LogicalCores = 12 },
    ];

    public IEnumerable<MessageSeverity> CreateMessageSeverities() =>
    [
        new MessageSeverity { MessageSeverityId = 1, SeverityName = "Error", SeverityDescription = "Critical compilation error" },
        new MessageSeverity { MessageSeverityId = 2, SeverityName = "Warning", SeverityDescription = "Non-blocking issue" },
        new MessageSeverity { MessageSeverityId = 3, SeverityName = "Info", SeverityDescription = "Informational notification" },
    ];

    public IEnumerable<ExecutionResult> CreateExecutionResults() =>
    [
        new ExecutionResult { ExecutionResultId = 1, ResultName = "Passed", ResultDescription = "Execution completed successfully" },
        new ExecutionResult { ExecutionResultId = 2, ResultName = "Failed", ResultDescription = "Execution encountered errors" },
        new ExecutionResult { ExecutionResultId = 3, ResultName = "Aborted", ResultDescription = "Execution was aborted" },
        new ExecutionResult { ExecutionResultId = 4, ResultName = "InProgress", ResultDescription = "Execution is currently running" },
    ];

    public IEnumerable<ProcessStage> CreateProcessStages() =>
    [
        new ProcessStage { ProcessStageId = 1, StageName = "Compile" },
        new ProcessStage { ProcessStageId = 2, StageName = "UnitTest" },
        new ProcessStage { ProcessStageId = 3, StageName = "CodeAnalysis" },
        new ProcessStage { ProcessStageId = 4, StageName = "Deploy" },
    ];

    public IEnumerable<ErrorCode> CreateErrorCodes() =>
    [
        new ErrorCode { ErrorCodeId = 1, CodeValue = "ERR001", CodeDescription = "Type resolution failed" },
        new ErrorCode { ErrorCodeId = 2, CodeValue = "ERR002", CodeDescription = "Undefined symbol reference" },
        new ErrorCode { ErrorCodeId = 3, CodeValue = "ERR003", CodeDescription = "Syntax error detected" },
        new ErrorCode { ErrorCodeId = 4, CodeValue = "ERR004", CodeDescription = "Unused variable declared" },
        new ErrorCode { ErrorCodeId = 5, CodeValue = "ERR005", CodeDescription = "Unused assignment" },
    ];

    public ServerConfiguration CreateServerConfiguration() =>
        new()
        {
            ProcessorModelId = 1,
            MemoryCapacityGb = 32.00m,
            SystemEnvironmentId = 1,
        };

    public IEnumerable<BenchmarkTest> CreateBenchmarkTests() =>
    [
        new BenchmarkTest { TestDescription = "Matrix Computation 2000x2000" },
        new BenchmarkTest { TestDescription = "Fibonacci Recursive (n=45)" },
        new BenchmarkTest { TestDescription = "Array Sort (10M elements)" },
        new BenchmarkTest { TestDescription = "Network Requests (1000)" },
        new BenchmarkTest { TestDescription = "Database Queries (10K records)" },
    ];
}