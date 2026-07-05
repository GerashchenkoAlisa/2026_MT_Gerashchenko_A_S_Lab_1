// <copyright file="IDataRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Linq.Expressions;
using Entities;

namespace Repository;

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