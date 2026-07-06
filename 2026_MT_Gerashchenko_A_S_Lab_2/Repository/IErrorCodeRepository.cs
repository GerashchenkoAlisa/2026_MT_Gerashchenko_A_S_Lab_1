using System.Threading.Tasks;
using Data;
using Entities;

namespace Repository;

public interface IErrorCodeRepository : IDataRepository<ErrorCode>
{
    Task<ErrorCode?> GetByCodeValueAsync(string codeValue);
}