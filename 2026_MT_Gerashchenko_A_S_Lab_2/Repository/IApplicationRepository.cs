using System.Threading.Tasks;
using Data;
using Entities;

namespace Repository;

public interface IApplicationRepository : IDataRepository<Application>
{
    Task<Application?> GetByRepositoryPathAsync(string repositoryPath);

    Task<Application?> GetWithBuildHistoryAsync(int id);
}