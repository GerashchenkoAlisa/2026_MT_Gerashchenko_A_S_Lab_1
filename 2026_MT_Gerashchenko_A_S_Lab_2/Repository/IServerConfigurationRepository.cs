using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using System.Threading.Tasks;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public interface IServerConfigurationRepository : IDataRepository<ServerConfiguration>
{
    Task<ServerConfiguration?> GetDefaultServerAsync();

    Task<ServerConfiguration?> GetWithProcessorAsync(int id);
}
