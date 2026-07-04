// <copyright file="RectMatrix.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Numerics;
using MTLAB3.MatrixLib;

namespace MTLAB3.MatrixLib;

public sealed class RectMatrix<T>(int rows, int cols) : MatrixBase<T>
    where T : INumber<T>
{
    private readonly T[,] data = new T[rows, cols];

    public override int Rows { get; } = rows;

    public override int Cols { get; } = cols;

    public override T this[int row, int col]
    {
        get => this.data[row, col];
        set => this.data[row, col] = value;
    }

    protected override IMatrix<T> CreateSameType(int rows, int cols) =>
        new RectMatrix<T>(rows, cols);

    public static IMatrix<T> LoadFromBinaryFile(string path)
    {
        return LoadFromBinaryFileCore(path, (r, c) => new RectMatrix<T>(r, c));
    }
}
