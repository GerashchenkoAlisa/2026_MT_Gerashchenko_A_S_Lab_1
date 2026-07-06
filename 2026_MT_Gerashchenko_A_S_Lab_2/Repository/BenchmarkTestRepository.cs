using System.Threading.Tasks;
using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using Repository;

namespace Repository;

public class BenchmarkTestRepository(BuildSystemDbContext context)
    : BaseDataRepository<BenchmarkTest>(context), IBenchmarkTestRepository
{
    public async Task<BenchmarkTest?> GetByTestDescriptionAsync(string testDescription)
    {
        return await this.DbSet.FirstOrDefaultAsync(bt => bt.TestDescription == testDescription).ConfigureAwait(false);
    }
}
