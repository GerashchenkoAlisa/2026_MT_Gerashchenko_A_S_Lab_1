using Entities;
using System.Collections.Generic;

namespace Factories;

public interface ISystemDataFactory
{
    IEnumerable<SystemEnvironment> CreateSystemEnvironments();

    IEnumerable<ProcessorModel> CreateProcessorModels();

    IEnumerable<MessageSeverity> CreateMessageSeverities();

    IEnumerable<ExecutionResult> CreateExecutionResults();

    IEnumerable<ProcessStage> CreateProcessStages();

    IEnumerable<ErrorCode> CreateErrorCodes();

    ServerConfiguration CreateServerConfiguration();

    IEnumerable<BenchmarkTest> CreateBenchmarkTests();
}