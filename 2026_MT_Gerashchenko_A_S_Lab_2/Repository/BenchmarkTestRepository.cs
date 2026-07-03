using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using Microsoft.EntityFrameworkCore;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public class BenchmarkTestRepository(BuildSystemDbContext context)
    : BaseDataRepository<BenchmarkTest>(context), IBenchmarkTestRepository
{
    public async Task<BenchmarkTest?> GetByTestDescriptionAsync(string testDescription)
    {
        return await this.DbSet.FirstOrDefaultAsync(bt => bt.TestDescription == testDescription).ConfigureAwait(false);
    }
}
