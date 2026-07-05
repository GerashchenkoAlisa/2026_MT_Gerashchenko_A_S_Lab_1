// <copyright file="IBenchmarkTestRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Entities;

namespace Repository;

public interface IBenchmarkTestRepository : IDataRepository<BenchmarkTest>
{
    Task<BenchmarkTest?> GetByTestDescriptionAsync(string testDescription);
}
