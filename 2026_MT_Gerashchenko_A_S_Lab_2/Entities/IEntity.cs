// <copyright file="IEntity.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Entities;

public interface IEntity<out TKey>
    where TKey : struct
{
    TKey Id { get; }

    string ToLogString(string val = "");
}