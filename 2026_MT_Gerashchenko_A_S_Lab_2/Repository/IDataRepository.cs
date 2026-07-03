using System.Linq.Expressions;
using _2026_MT_Gerashchenko_A_S_Lab_2.Entities;

namespace _2026_MT_Gerashchenko_A_S_Lab_2.Repository;

public interface IDataRepository<T>
    where T : BaseEntity<int>
{
    Task<T?> GetByIdAsync(int id);

    Task<IEnumerable<T>> GetAllAsync();

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    Task AddAsync(T entity);

    Task UpdateAsync(T entity);

    Task DeleteAsync(T entity);

    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
}