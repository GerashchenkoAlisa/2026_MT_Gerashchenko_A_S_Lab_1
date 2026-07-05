// <copyright file="IErrorCodeRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using Entities;

namespace Repository;

public interface IErrorCodeRepository : IDataRepository<ErrorCode>
{
    Task<ErrorCode?> GetByCodeValueAsync(string codeValue);
}