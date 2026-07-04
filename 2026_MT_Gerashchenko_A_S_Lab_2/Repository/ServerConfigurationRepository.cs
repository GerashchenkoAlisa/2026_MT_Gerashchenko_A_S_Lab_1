using Data;
using Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Repository;

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