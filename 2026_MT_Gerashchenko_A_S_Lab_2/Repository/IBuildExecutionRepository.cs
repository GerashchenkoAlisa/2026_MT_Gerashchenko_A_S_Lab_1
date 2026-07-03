using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public interface IBuildExecutionRepository : IDataRepository<BuildExecution>
{
    Task<IEnumerable<BuildExecution>> GetByApplicationIdAsync(int applicationId);

    Task<IEnumerable<BuildExecution>> GetByProcessStageIdAsync(int processStageId);

    Task<BuildExecution?> GetWithMessagesAsync(int id);
}