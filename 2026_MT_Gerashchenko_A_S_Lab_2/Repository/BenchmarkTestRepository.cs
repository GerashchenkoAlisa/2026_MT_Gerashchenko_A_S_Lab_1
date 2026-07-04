using Data;
using Entities;
using Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Repository;

public class BenchmarkTestRepository(BuildSystemDbContext context)
    : BaseDataRepository<BenchmarkTest>(context), IBenchmarkTestRepository
{
    public async Task<BenchmarkTest?> GetByTestDescriptionAsync(string testDescription)
    {
        return await this.DbSet.FirstOrDefaultAsync(bt => bt.TestDescription == testDescription).ConfigureAwait(false);
    }
}
