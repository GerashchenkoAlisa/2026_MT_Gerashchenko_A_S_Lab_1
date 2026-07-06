using System.Threading.Tasks;
using Entities;

namespace Repository;

public interface IServerConfigurationRepository : IDataRepository<ServerConfiguration>
{
    Task<ServerConfiguration?> GetDefaultServerAsync();

    Task<ServerConfiguration?> GetWithProcessorAsync(int id);
}
