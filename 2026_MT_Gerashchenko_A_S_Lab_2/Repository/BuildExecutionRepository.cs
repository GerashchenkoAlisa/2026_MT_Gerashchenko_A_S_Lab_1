using Data;
using Entities;
using Repository;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository;

public class BuildExecutionRepository(BuildSystemDbContext context)
    : BaseDataRepository<BuildExecution>(context), IBuildExecutionRepository
{
    public async Task<IEnumerable<BuildExecution>> GetByApplicationIdAsync(int applicationId)
    {
        return await this.DbSet
            .Where(be => be.ApplicationId == applicationId)
            .OrderByDescending(be => be.ExecutionStartTime)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<BuildExecution>> GetByProcessStageIdAsync(int processStageId)
    {
        return await this.DbSet
            .Where(be => be.ProcessStageId == processStageId)
            .OrderByDescending(be => be.ExecutionStartTime)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<BuildExecution?> GetWithMessagesAsync(int id)
    {
        return await this.DbSet
            .Include(be => be.BuildMessages)
            .ThenInclude(bm => bm.MessageSeverity)
            .Include(be => be.BuildMessages)
            .ThenInclude(bm => bm.ErrorCode)
            .FirstOrDefaultAsync(be => be.BuildExecutionId == id)
            .ConfigureAwait(false);
    }
}
