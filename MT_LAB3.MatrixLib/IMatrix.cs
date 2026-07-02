using System.Numerics;

namespace MT_LAB3.MatrixLib;
public interface IMatrix<T> where T : INumber<T>
{
    int Rows { get; }
    int Cols { get; }
    T this[int row, int col] { get; set; }

    IMatrix<T> AddByRowsSequential(IMatrix<T> other);
    IMatrix<T> AddByRowsParallel(IMatrix<T> other);
    IMatrix<T> AddByColumnsSequential(IMatrix<T> other);
    IMatrix<T> AddByColumnsParallel(IMatrix<T> other);

    IMatrix<T> MultiplySequential(IMatrix<T> other);
    IMatrix<T> MultiplyParallel(IMatrix<T> other);
    IMatrix<T> MultiplyOptimSequential(IMatrix<T> other);
    IMatrix<T> MultiplyOptimParallel(IMatrix<T> other);
    IMatrix<T> MultiplyNaiveSequential(IMatrix<T> other);
    IMatrix<T> MultiplyNaiveParallel(IMatrix<T> other);
}