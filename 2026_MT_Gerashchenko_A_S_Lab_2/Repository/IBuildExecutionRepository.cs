// <copyright file="IBuildExecutionRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Entities;

namespace Repository;

public interface IBuildExecutionRepository : IDataRepository<BuildExecution>
{
    Task<IEnumerable<BuildExecution>> GetByApplicationIdAsync(int applicationId);

    Task<IEnumerable<BuildExecution>> GetByProcessStageIdAsync(int processStageId);

    Task<BuildExecution?> GetWithMessagesAsync(int id);
}