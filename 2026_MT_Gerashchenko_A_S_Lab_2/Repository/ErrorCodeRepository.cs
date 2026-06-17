using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using _2026_MT_Gerashchenko_A_S_Lab_2.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

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