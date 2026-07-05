// <copyright file="IBuildMessageRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Entities;

namespace Repository;

public interface IBuildMessageRepository : IDataRepository<BuildMessage>
{
    Task<IEnumerable<BuildMessage>> GetByBuildExecutionIdAsync(int buildExecutionId);

    Task<IEnumerable<BuildMessage>> GetErrorsByBuildExecutionIdAsync(int buildExecutionId);

    Task<IEnumerable<BuildMessage>> GetWarningsByBuildExecutionIdAsync(int buildExecutionId);
}