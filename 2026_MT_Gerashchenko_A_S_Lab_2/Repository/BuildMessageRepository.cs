using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Repository;

public class BuildMessageRepository(BuildSystemDbContext context)
    : BaseDataRepository<BuildMessage>(context), IBuildMessageRepository
{
    public async Task<IEnumerable<BuildMessage>> GetByBuildExecutionIdAsync(int buildExecutionId)
    {
        return await this.DbSet
            .Where(bm => bm.BuildExecutionId == buildExecutionId)
            .OrderBy(bm => bm.MessageTimestamp)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<BuildMessage>> GetErrorsByBuildExecutionIdAsync(int buildExecutionId)
    {
        return await this.DbSet
            .Where(bm => bm.BuildExecutionId == buildExecutionId
                      && bm.MessageSeverity.SeverityName == "Error")
            .OrderBy(bm => bm.MessageTimestamp)
            .ToListAsync().ConfigureAwait(false);
    }

    public async Task<IEnumerable<BuildMessage>> GetWarningsByBuildExecutionIdAsync(int buildExecutionId)
    {
        return await this.DbSet
            .Where(bm => bm.BuildExecutionId == buildExecutionId
                      && bm.MessageSeverity.SeverityName == "Warning")
            .OrderBy(bm => bm.MessageTimestamp)
            .ToListAsync().ConfigureAwait(false);
    }
}