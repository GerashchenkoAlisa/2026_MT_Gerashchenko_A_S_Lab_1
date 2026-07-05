// <copyright file="IServerConfigurationRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Entities;

namespace Repository;

public interface IServerConfigurationRepository : IDataRepository<ServerConfiguration>
{
    Task<ServerConfiguration?> GetDefaultServerAsync();

    Task<ServerConfiguration?> GetWithProcessorAsync(int id);
}
