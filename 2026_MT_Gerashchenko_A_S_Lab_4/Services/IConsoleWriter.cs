namespace _2026_MT_Gerashchenko_A_S_Lab_4.Services;
public interface IConsoleWriter
{
    Task WriteLineAsync(string message, CancellationToken ct = default);

    Task WriteErrorLineAsync(string message, CancellationToken ct = default);
}