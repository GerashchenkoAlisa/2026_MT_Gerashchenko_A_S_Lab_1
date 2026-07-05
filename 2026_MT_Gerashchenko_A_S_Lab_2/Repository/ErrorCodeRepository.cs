// <copyright file="ErrorCodeRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Data;
using Entities;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ErrorCodeRepository(BuildSystemDbContext context)
    : BaseDataRepository<ErrorCode>(context), IErrorCodeRepository
{
    public async Task<ErrorCode?> GetByCodeValueAsync(string codeValue)
    {
        return await this.DbSet
            .FirstOrDefaultAsync(ec => ec.CodeValue == codeValue)
            .ConfigureAwait(false);
    }
}