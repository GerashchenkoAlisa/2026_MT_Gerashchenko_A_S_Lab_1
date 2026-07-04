namespace Services;
public interface IConsoleWriter
{
    Task WriteLineAsync(string message, CancellationToken ct = default);

    Task WriteErrorLineAsync(string message, CancellationToken ct = default);
}