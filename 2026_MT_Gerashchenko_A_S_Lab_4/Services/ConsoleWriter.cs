namespace _2026_MT_Gerashchenko_A_S_Lab_4.Services;
public sealed class ConsoleWriter : IConsoleWriter
{
    public Task WriteLineAsync(string message, CancellationToken ct = default) =>
        Console.Out.WriteLineAsync(message.AsMemory(), ct);

    public Task WriteErrorLineAsync(string message, CancellationToken ct = default) =>
        Console.Error.WriteLineAsync(message.AsMemory(), ct);
}