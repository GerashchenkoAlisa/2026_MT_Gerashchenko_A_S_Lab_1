/// <summary>
/// Provides repository methods for <see cref="BuildExecution"/> entities.
/// </summary>
public class BuildExecutionRepository : BaseDataRepository<BuildExecution>, IBuildExecutionRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BuildExecutionRepository"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public BuildExecutionRepository(BuildSystemDbContext context)
        : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BuildExecution>> GetByApplicationIdAsync(int applicationId)
    {
        return await this.DbSet
            .Where(be => be.ApplicationId == applicationId)
            .OrderByDescending(be => be.ExecutionStartTime)
            .ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<BuildExecution>> GetByProcessStageIdAsync(int processStageId)
    {
        return await this.DbSet
            .Where(be => be.ProcessStageId == processStageId)
            .OrderByDescending(be => be.ExecutionStartTime)
            .ToListAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
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