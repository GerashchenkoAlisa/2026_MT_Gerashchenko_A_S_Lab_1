using System.Collections.Generic;
using System.Threading.Tasks;
using Entities;

namespace Repository;

public interface IBuildExecutionRepository : IDataRepository<BuildExecution>
{
    Task<IEnumerable<BuildExecution>> GetByApplicationIdAsync(int applicationId);

    Task<IEnumerable<BuildExecution>> GetByProcessStageIdAsync(int processStageId);

    Task<BuildExecution?> GetWithMessagesAsync(int id);
}