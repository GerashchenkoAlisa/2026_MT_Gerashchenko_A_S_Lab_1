using Entities;
using System.Threading.Tasks;

namespace Repository;

public interface IBenchmarkTestRepository : IDataRepository<BenchmarkTest>
{
    Task<BenchmarkTest?> GetByTestDescriptionAsync(string testDescription);
}
