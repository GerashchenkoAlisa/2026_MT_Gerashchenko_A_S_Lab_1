using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Factories;

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