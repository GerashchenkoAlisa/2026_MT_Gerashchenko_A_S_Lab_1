using System.Collections.Generic;
using System.Threading.Tasks;
using Data;
using Entities;

namespace Repository;

public interface IBuildMessageRepository : IDataRepository<BuildMessage>
{
    Task<IEnumerable<BuildMessage>> GetByBuildExecutionIdAsync(int buildExecutionId);

    Task<IEnumerable<BuildMessage>> GetErrorsByBuildExecutionIdAsync(int buildExecutionId);

    Task<IEnumerable<BuildMessage>> GetWarningsByBuildExecutionIdAsync(int buildExecutionId);
}