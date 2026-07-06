using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Repository;

public class ErrorCodeRepository(Data.BuildSystemDbContext context)
    : BaseDataRepository<ErrorCode>(context), IErrorCodeRepository
{
    public async Task<ErrorCode?> GetByCodeValueAsync(string codeValue)
    {
        return await this.DbSet
            .FirstOrDefaultAsync(ec => ec.CodeValue == codeValue)
            .ConfigureAwait(false);
    }
}