// <copyright file="BenchmarkTestRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class BenchmarkTestRepository(BuildSystemDbContext context)
    : BaseDataRepository<BenchmarkTest>(context), IBenchmarkTestRepository
{
    public async Task<BenchmarkTest?> GetByTestDescriptionAsync(string testDescription)
    {
        return await this.DbSet.FirstOrDefaultAsync(bt => bt.TestDescription == testDescription).ConfigureAwait(false);
    }
}
