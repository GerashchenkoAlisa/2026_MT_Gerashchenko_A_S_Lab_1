using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public interface IBuildMessageRepository : IDataRepository<BuildMessage>
{
    Task<IEnumerable<BuildMessage>> GetByBuildExecutionIdAsync(int buildExecutionId);

    Task<IEnumerable<BuildMessage>> GetErrorsByBuildExecutionIdAsync(int buildExecutionId);

    Task<IEnumerable<BuildMessage>> GetWarningsByBuildExecutionIdAsync(int buildExecutionId);
}