using System.Threading.Tasks;
using Entities;

namespace Repository;

public interface IBenchmarkTestRepository : IDataRepository<BenchmarkTest>
{
    Task<BenchmarkTest?> GetByTestDescriptionAsync(string testDescription);
}
