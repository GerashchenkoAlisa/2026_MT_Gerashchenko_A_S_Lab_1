// <copyright file="MatrixExtensions.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Numerics;
using MTLAB3.MatrixLib;

namespace MTLAB3.MatrixLib;
public static class MatrixExtensions
{
    public static void SaveToBinaryFile<T>(this IMatrix<T> matrix, string path)
        where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(matrix);

        using var writer = new BinaryWriter(File.OpenWrite(path));
        writer.Write(matrix.Rows);
        writer.Write(matrix.Cols);
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Cols; j++)
            {
                WriteValue(writer, matrix[i, j]);
            }
        }
    }

    public static bool MatrixEquals<T>(this IMatrix<T> a, IMatrix<T> b, T tolerance)
        where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        if (a is MatrixBase<T> baseA)
        {
            return baseA.MatrixEquals(b, tolerance);
        }

        if (a.Rows != b.Rows || a.Cols != b.Cols)
        {
            return false;
        }

        for (int i = 0; i < a.Rows; i++)
        {
            for (int j = 0; j < a.Cols; j++)
            {
                T diff = a[i, j] - b[i, j];
                if (diff > tolerance || diff < -tolerance)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public static bool MatrixEquals<T>(this IMatrix<T> a, IMatrix<T> b)
        where T : INumber<T> =>
        a.MatrixEquals(b, T.Zero);

    public static void Fill<T>(this IMatrix<T> matrix, Func<int, int, T> valueSelector)
        where T : INumber<T>
    {
        ArgumentNullException.ThrowIfNull(matrix);
        ArgumentNullException.ThrowIfNull(valueSelector);
        for (int i = 0; i < matrix.Rows; i++)
        {
            for (int j = 0; j < matrix.Cols; j++)
            {
                matrix[i, j] = valueSelector(i, j);
            }
        }
    }

    private static void WriteValue<T>(BinaryWriter writer, T value)
        where T : INumber<T>
    {
        if (typeof(T) == typeof(int))
        {
            writer.Write(int.CreateChecked(value));
            return;
        }

        if (typeof(T) == typeof(double))
        {
            writer.Write(double.CreateChecked(value));
            return;
        }

        if (typeof(T) == typeof(float))
        {
            writer.Write(float.CreateChecked(value));
            return;
        }

        if (typeof(T) == typeof(decimal))
        {
            writer.Write(decimal.CreateChecked(value));
            return;
        }

        if (typeof(T) == typeof(long))
        {
            writer.Write(long.CreateChecked(value));
            return;
        }

        throw new NotSupportedException($"Binary writing for type {typeof(T).Name} is not supported.");
    }
}
