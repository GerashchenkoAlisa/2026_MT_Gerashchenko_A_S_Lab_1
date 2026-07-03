using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using Microsoft.EntityFrameworkCore;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public class ServerConfigurationRepository(BuildSystemDbContext context)
    : BaseDataRepository<ServerConfiguration>(context), IServerConfigurationRepository
{
    public async Task<ServerConfiguration?> GetDefaultServerAsync()
    {
        return await this.DbSet
            .Include(sc => sc.ProcessorModel)
            .FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public async Task<ServerConfiguration?> GetWithProcessorAsync(int id)
    {
        return await this.DbSet
            .Include(sc => sc.ProcessorModel)
            .FirstOrDefaultAsync(sc => sc.ServerConfigurationId == id).ConfigureAwait(false);
    }
}