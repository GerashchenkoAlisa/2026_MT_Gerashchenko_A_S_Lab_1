using Entities;
using System.Threading.Tasks;

namespace Repository;

public interface IServerConfigurationRepository : IDataRepository<ServerConfiguration>
{
    Task<ServerConfiguration?> GetDefaultServerAsync();

    Task<ServerConfiguration?> GetWithProcessorAsync(int id);
}
