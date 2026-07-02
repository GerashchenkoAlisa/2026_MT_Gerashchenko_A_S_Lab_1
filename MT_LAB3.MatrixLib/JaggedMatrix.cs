using System.Numerics;
using MT_LAB3.MatrixLib;

namespace MT_LAB3.MatrixLib;
public sealed class JaggedMatrix<T> : MatrixBase<T> where T : INumber<T>
{
    private readonly T[][] _data;

    public override int Rows { get; }
    public override int Cols { get; }

    public override T this[int row, int col]
    {
        get => _data[row][col];
        set => _data[row][col] = value;
    }

    public JaggedMatrix(int rows, int cols)
    {
        Rows = rows;
        Cols = cols;
        _data = new T[rows][];
        for (int i = 0; i < rows; i++)
            _data[i] = new T[cols];
    }

    protected override IMatrix<T> CreateSameType(int rows, int cols) =>
        new JaggedMatrix<T>(rows, cols);

    public static IMatrix<T> LoadFromBinaryFile(string path) =>
        LoadFromBinaryFileCore(path, (r, c) => new JaggedMatrix<T>(r, c));
}