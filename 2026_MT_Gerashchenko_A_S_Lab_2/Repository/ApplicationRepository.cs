using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using _2026_MT_Gerashchenko_A_S_Lab_2.Repository;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

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
