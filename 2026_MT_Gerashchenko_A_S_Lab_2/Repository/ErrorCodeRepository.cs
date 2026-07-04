using   Data;
using   Entities;
using Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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