public interface ILoggerService
{
    void LogInfo(string message);
    void LogSuccess(string message);
    void LogError(string message);
    void LogWarning(string message);
}