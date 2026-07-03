using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public interface IServerConfigurationRepository : IDataRepository<ServerConfiguration>
{
    Task<ServerConfiguration?> GetDefaultServerAsync();

    Task<ServerConfiguration?> GetWithProcessorAsync(int id);
}
