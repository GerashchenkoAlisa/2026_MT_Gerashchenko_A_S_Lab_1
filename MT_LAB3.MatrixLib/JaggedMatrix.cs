using System.Numerics;

namespace MTLAB3.MatrixLib;
public sealed class JaggedMatrix<T> : MatrixBase<T>
    where T : INumber<T>
{
    private readonly T[][] data;

    public JaggedMatrix(int rows, int cols)
    {
        this.Rows = rows;
        this.Cols = cols;
        this.data = new T[rows][];
        for (int i = 0; i < rows; i++)
        {
            this.data[i] = new T[cols];
        }
    }

    public override int Rows { get; }

    public override int Cols { get; }

    public override T this[int row, int col]
    {
        get => this.data[row][col];
        set => this.data[row][col] = value;
    }

    public static IMatrix<T> LoadFromBinaryFile(string path) =>
        LoadFromBinaryFileCore(path, (r, c) => new JaggedMatrix<T>(r, c));

    protected override IMatrix<T> CreateSameType(int rows, int cols) =>
        new JaggedMatrix<T>(rows, cols);
}