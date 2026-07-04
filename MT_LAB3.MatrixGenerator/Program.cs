// <copyright file="Program.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using System.Numerics;
using System.Security.Cryptography;
using MatrixLib;
using MTLAB3.MatrixLib;

namespace MTLAB3.MatrixGenerator;

internal static class Program
{
    private static readonly string OutputDir = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "matrices");

    private static void Main()
    {
        Directory.CreateDirectory(OutputDir);

        Console.WriteLine($"Output directory: {Path.GetFullPath(OutputDir)}");
        Console.WriteLine();

        GenerateFixed10x10Int();
        GenerateFixed10x10Double();

        GenerateRandom<int>(500, 500, "matrix_500x500_int.bin");
        GenerateRandom<double>(500, 500, "matrix_500x500_double.bin");

        GenerateRandom<int>(2000, 2000, "matrix_2000x2000_int.bin");
        GenerateRandom<double>(2000, 2000, "matrix_2000x2000_double.bin");
    }

    private static void GenerateFixed10x10Int()
    {
        var m = new RectMatrix<int>(10, 10);
        m.Fill((i, j) => (i * 10) + j + 1);
        SaveMatrix(m, "matrix_10x10_int.bin");
    }

    private static void GenerateFixed10x10Double()
    {
        var m = new RectMatrix<double>(10, 10);
        m.Fill((i, j) => (i + 1) + ((j + 1) * 0.1));
        SaveMatrix(m, "matrix_10x10_double.bin");
    }

    private static void GenerateRandom<T>(int rows, int cols, string filename)
        where T : INumber<T>
    {
        var m = new RectMatrix<T>(rows, cols);
        m.Fill((_, _) => GenerateRandomValue<T>());
        SaveMatrix(m, filename);
    }

    private static T GenerateRandomValue<T>()
        where T : INumber<T>
    {
        if (typeof(T) == typeof(int))
        {
            Span<byte> bytes = stackalloc byte[4];
            RandomNumberGenerator.Fill(bytes);
            int value = BitConverter.ToInt32(bytes);
            value = Math.Abs(value % 99) + 1; 
            return T.CreateChecked(value);
        }

        if (typeof(T) == typeof(double))
        {
            Span<byte> bytes = stackalloc byte[8];
            RandomNumberGenerator.Fill(bytes);
            ulong ul = BitConverter.ToUInt64(bytes);
            double d = (ul / (double)ulong.MaxValue) * 100.0;
            return T.CreateChecked(d);
        }

        if (typeof(T) == typeof(float))
        {
            Span<byte> bytes = stackalloc byte[4];
            RandomNumberGenerator.Fill(bytes);
            uint ui = BitConverter.ToUInt32(bytes);
            float f = (ui / (float)uint.MaxValue) * 100.0f;
            return T.CreateChecked(f);
        }

        if (typeof(T) == typeof(long))
        {
            Span<byte> bytes = stackalloc byte[8];
            RandomNumberGenerator.Fill(bytes);
            long value = BitConverter.ToInt64(bytes);
            value = Math.Abs(value % 99) + 1; 
            return T.CreateChecked(value);
        }

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
