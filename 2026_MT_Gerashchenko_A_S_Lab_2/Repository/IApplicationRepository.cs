using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public interface IApplicationRepository : IDataRepository<Application>
{
    Task<Application?> GetByRepositoryPathAsync(string repositoryPath);

    Task<Application?> GetWithBuildHistoryAsync(int id);
}