// <copyright file="IApplicationRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Entities;

namespace Repository;

public interface IApplicationRepository : IDataRepository<Application>
{
    Task<Application?> GetByRepositoryPathAsync(string repositoryPath);

    Task<Application?> GetWithBuildHistoryAsync(int id);
}