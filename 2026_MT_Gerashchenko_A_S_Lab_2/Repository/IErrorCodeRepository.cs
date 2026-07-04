using Data;
using Entities;
using System.Threading.Tasks;

namespace Repository;

public interface IErrorCodeRepository : IDataRepository<ErrorCode>
{
    Task<ErrorCode?> GetByCodeValueAsync(string codeValue);
}