using System.Numerics;
using MT_LAB3.MatrixLib;

namespace MT_LAB3.MatrixGenerator;
internal static class Program
{
    private static readonly string OutputDir = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "matrices");

    static void Main()
    {
        Directory.CreateDirectory(OutputDir);

        Console.WriteLine("Matrix Binary File Generator");
        Console.WriteLine($"Output directory: {Path.GetFullPath(OutputDir)}");
        Console.WriteLine();

        Console.WriteLine("Generating 10x10 (fixed values)...");
        GenerateFixed10x10Int();
        GenerateFixed10x10Double();

        Console.WriteLine("Generating 500x500 (random)...");
        GenerateRandom<int>(500, 500, "matrix_500x500_int.bin");
        GenerateRandom<double>(500, 500, "matrix_500x500_double.bin");

        Console.WriteLine("Generating 2000x2000 (random)...");
        GenerateRandom<int>(2000, 2000, "matrix_2000x2000_int.bin");
        GenerateRandom<double>(2000, 2000, "matrix_2000x2000_double.bin");

        Console.WriteLine("\nAll files generated successfully.");
    }

    private static void GenerateFixed10x10Int()
    {
        var m = new RectMatrix<int>(10, 10);
        m.Fill((i, j) => i * 10 + j + 1);
        SaveMatrix(m, "matrix_10x10_int.bin");
    }

    private static void GenerateFixed10x10Double()
    {
        var m = new RectMatrix<double>(10, 10);
        m.Fill((i, j) => (i + 1) + (j + 1) * 0.1);
        SaveMatrix(m, "matrix_10x10_double.bin");
    }

    private static void GenerateRandom<T>(int rows, int cols, string filename)
        where T : INumber<T>
    {
        var rng = new Random(42);
        var m = new RectMatrix<T>(rows, cols);
        m.Fill((_, _) => GenerateRandomValue<T>(rng));
        SaveMatrix(m, filename);
    }

    private static T GenerateRandomValue<T>(Random rng) where T : INumber<T>
    {
        if (typeof(T) == typeof(int)) return T.CreateChecked(rng.Next(1, 100));
        if (typeof(T) == typeof(double)) return T.CreateChecked(rng.NextDouble() * 100.0);
        if (typeof(T) == typeof(float)) return T.CreateChecked((float)(rng.NextDouble() * 100.0));
        if (typeof(T) == typeof(long)) return T.CreateChecked(rng.NextInt64(1, 100));
        throw new NotSupportedException($"Random generation for {typeof(T).Name} is not supported.");
    }

    private static void SaveMatrix<T>(RectMatrix<T> matrix, string filename)
        where T : INumber<T>
    {
        string path = Path.Combine(OutputDir, filename);
        matrix.SaveToBinaryFile(path);
        Console.WriteLine($"  Saved: {filename}");
    }
}