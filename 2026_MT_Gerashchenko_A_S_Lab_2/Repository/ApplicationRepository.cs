// <copyright file="ApplicationRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ApplicationRepository(BuildSystemDbContext context)
    : BaseDataRepository<Application>(context), IApplicationRepository
{
    public async Task<Application?> GetByRepositoryPathAsync(string repositoryPath)
    {
        return await this.DbSet.FirstOrDefaultAsync(a => a.RepositoryPath == repositoryPath).ConfigureAwait(false);
    }

    public async Task<Application?> GetWithBuildHistoryAsync(int id)
    {
        return await this.DbSet
            .Include(a => a.BuildExecutions)
            .ThenInclude(be => be.ProcessStage)
            .FirstOrDefaultAsync(a => a.Id == id).ConfigureAwait(false);
    }
}
