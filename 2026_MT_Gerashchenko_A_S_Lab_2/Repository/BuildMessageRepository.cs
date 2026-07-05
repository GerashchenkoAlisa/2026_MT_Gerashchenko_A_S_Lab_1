// <copyright file="BuildMessageRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;

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