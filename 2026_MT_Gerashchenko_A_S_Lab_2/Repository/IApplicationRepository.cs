using Data;
using Entities;
using System.Threading.Tasks;

namespace Repository;

public interface IApplicationRepository : IDataRepository<Application>
{
    Task<Application?> GetByRepositoryPathAsync(string repositoryPath);

    Task<Application?> GetWithBuildHistoryAsync(int id);
}