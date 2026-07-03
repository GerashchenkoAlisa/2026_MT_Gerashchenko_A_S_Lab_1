using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using Microsoft.EntityFrameworkCore;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

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