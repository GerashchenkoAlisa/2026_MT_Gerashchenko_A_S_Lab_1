// <copyright file="MatrixBase.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Numerics;

namespace MTLAB3.MatrixLib;
public abstract class MatrixBase<T> : IMatrix<T>
    where T : INumber<T>
{
    public abstract int Rows { get; }

    public abstract int Cols { get; }

    public abstract T this[int row, int col] { get; set; }

    public IMatrix<T> AddByRowsSequential(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateSameDimensions(other);
        var result = this.CreateSameType();
        for (int i = 0; i < this.Rows; i++)
        {
            for (int j = 0; j < this.Cols; j++)
            {
                result[i, j] = this[i, j] + other[i, j];
            }
        }

        return result;
    }

    public IMatrix<T> AddByRowsParallel(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateSameDimensions(other);
        var result = this.CreateSameType();
        Parallel.For(0, this.Rows, i =>
        {
            for (int j = 0; j < this.Cols; j++)
            {
                result[i, j] = this[i, j] + other[i, j];
            }
        });
        return result;
    }

    public IMatrix<T> AddByColumnsSequential(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateSameDimensions(other);
        var result = this.CreateSameType();
        for (int j = 0; j < this.Cols; j++)
        {
            for (int i = 0; i < this.Rows; i++)
            {
                result[i, j] = this[i, j] + other[i, j];
            }
        }

        return result;
    }

    public IMatrix<T> AddByColumnsParallel(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateSameDimensions(other);
        var result = this.CreateSameType();
        Parallel.For(0, this.Cols, j =>
        {
            for (int i = 0; i < this.Rows; i++)
            {
                result[i, j] = this[i, j] + other[i, j];
            }
        });
        return result;
    }

    public IMatrix<T> MultiplySequential(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateMultiplicationDimensions(other);
        var result = this.CreateSameType(this.Rows, other.Cols);
        for (int i = 0; i < this.Rows; i++)
        {
            for (int j = 0; j < other.Cols; j++)
            {
                T sum = T.Zero;
                for (int k = 0; k < this.Cols; k++)
                {
                    sum += this[i, k] * other[k, j];
                }

                result[i, j] = sum;
            }
        }

        return result;
    }

    public IMatrix<T> MultiplyParallel(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateMultiplicationDimensions(other);
        var result = this.CreateSameType(this.Rows, other.Cols);
        Parallel.For(0, this.Rows, i =>
        {
            for (int j = 0; j < other.Cols; j++)
            {
                T sum = T.Zero;
                for (int k = 0; k < this.Cols; k++)
                {
                    sum += this[i, k] * other[k, j];
                }

                result[i, j] = sum;
            }
        });
        return result;
    }

    public IMatrix<T> MultiplyOptimSequential(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateMultiplicationDimensions(other);
        var result = this.CreateSameType(this.Rows, other.Cols);
        for (int i = 0; i < this.Rows; i++)
        {
            for (int k = 0; k < this.Cols; k++)
            {
                T aik = this[i, k];
                for (int j = 0; j < other.Cols; j++)
                {
                    result[i, j] += aik * other[k, j];
                }
            }
        }

        return result;
    }

    public IMatrix<T> MultiplyOptimParallel(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateMultiplicationDimensions(other);
        var result = this.CreateSameType(this.Rows, other.Cols);
        Parallel.For(0, this.Rows, i =>
        {
            for (int k = 0; k < this.Cols; k++)
            {
                T aik = this[i, k];
                for (int j = 0; j < other.Cols; j++)
                {
                    result[i, j] += aik * other[k, j];
                }
            }
        });
        return result;
    }

    public IMatrix<T> MultiplyNaiveSequential(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateMultiplicationDimensions(other);
        var result = this.CreateSameType(this.Rows, other.Cols);
        for (int j = 0; j < other.Cols; j++)
        {
            for (int i = 0; i < this.Rows; i++)
            {
                T sum = T.Zero;
                for (int k = 0; k < this.Cols; k++)
                {
                    sum += this[i, k] * other[k, j];
                }

                result[i, j] = sum;
            }
        }

        return result;
    }

    public IMatrix<T> MultiplyNaiveParallel(IMatrix<T> other)
    {
        ArgumentNullException.ThrowIfNull(other);
        this.ValidateMultiplicationDimensions(other);
        var result = this.CreateSameType(this.Rows, other.Cols);
        Parallel.For(0, other.Cols, j =>
        {
            for (int i = 0; i < this.Rows; i++)
            {
                T sum = T.Zero;
                for (int k = 0; k < this.Cols; k++)
                {
                    sum += this[i, k] * other[k, j];
                }

                result[i, j] = sum;
            }
        });
        return result;
    }

    public bool MatrixEquals(IMatrix<T> other, T tolerance)
    {
        if (other is null || this.Rows != other.Rows || this.Cols != other.Cols)
        {
            return false;
        }

        for (int i = 0; i < this.Rows; i++)
        {
            for (int j = 0; j < this.Cols; j++)
            {
                T diff = this[i, j] - other[i, j];
                if (diff > tolerance || diff < -tolerance)
                {
                    return false;
                }
            }
        }

        return true;
    }

    protected static IMatrix<T> LoadFromBinaryFileCore(string path, Func<int, int, MatrixBase<T>> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        using var reader = new BinaryReader(File.OpenRead(path));
        int rows = reader.ReadInt32();
        int cols = reader.ReadInt32();
        var matrix = factory(rows, cols);
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                matrix[i, j] = ReadValue(reader);
            }
        }

        return matrix;
    }

    protected abstract IMatrix<T> CreateSameType(int rows, int cols);

    protected IMatrix<T> CreateSameType() => this.CreateSameType(this.Rows, this.Cols);

    private static T ReadValue(BinaryReader reader)
    {
        if (typeof(T) == typeof(int))
        {
            return T.CreateChecked(reader.ReadInt32());
        }

        if (typeof(T) == typeof(double))
        {
            return T.CreateChecked(reader.ReadDouble());
        }

        if (typeof(T) == typeof(float))
        {
            return T.CreateChecked(reader.ReadSingle());
        }

        if (typeof(T) == typeof(decimal))
        {
            return T.CreateChecked(reader.ReadDecimal());
        }

        if (typeof(T) == typeof(long))
        {
            return T.CreateChecked(reader.ReadInt64());
        }

        throw new NotSupportedException($"Binary reading for type {typeof(T).Name} is not supported.");
    }

    private void ValidateSameDimensions(IMatrix<T> other)
    {
        if (this.Rows != other.Rows || this.Cols != other.Cols)
        {
            throw new InvalidOperationException(
                $"Matrix dimensions must match: ({this.Rows}×{this.Cols}) vs ({other.Rows}×{other.Cols}).");
        }
    }

    private void ValidateMultiplicationDimensions(IMatrix<T> other)
    {
        if (this.Cols != other.Rows)
        {
            throw new InvalidOperationException(
                $"Incompatible dimensions for multiplication: cols={this.Cols} != rows={other.Rows}.");
        }
    }
}
