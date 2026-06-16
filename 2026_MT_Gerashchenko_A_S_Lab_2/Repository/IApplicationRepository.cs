using _2026_MT_Gerashchenko_A_S_Lab_2.Data;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;
using System.Threading.Tasks;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public interface IApplicationRepository : IDataRepository<Application>
{
    Task<Application?> GetByRepositoryPathAsync(string repositoryPath);

    Task<Application?> GetWithBuildHistoryAsync(int id);
}