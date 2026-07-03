using System.Numerics;
using MT_LAB3.MatrixLib;

namespace MT_LAB3.MatrixLib;
public sealed class FlatMatrix<T> : MatrixBase<T>
    where T : INumber<T>
{
    private readonly T[] _data;

    public override int Rows { get; }

    public override int Cols { get; }

    public override T this[int row, int col]
    {
        get => this._data[(row * this.Cols) + col];
        set => this._data[(row * this.Cols) + col] = value;
    }

    public FlatMatrix(int rows, int cols)
    {
        this.Rows = rows;
        this.Cols = cols;
        this._data = new T[rows * cols];
    }

    protected override IMatrix<T> CreateSameType(int rows, int cols) =>
        new FlatMatrix<T>(rows, cols);

    public static IMatrix<T> LoadFromBinaryFile(string path)
    {
        return LoadFromBinaryFileCore(path, (r, c) => new FlatMatrix<T>(r, c));
    }
}
