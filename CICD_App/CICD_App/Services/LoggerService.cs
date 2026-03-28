namespace CICD_App.Services;

public class LoggerService : ILoggerService
{
    private readonly string _logFilePath;
    private readonly object _lockObject = new object();

    public LoggerService(string targetDir)
    {
        string workDirName = Path.GetFileName(targetDir.TrimEnd(Path.DirectorySeparatorChar));
        string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        string logFileName = $"CICD_{workDirName}_{timestamp}.log";
        _logFilePath = Path.Combine(targetDir, logFileName);

        Directory.CreateDirectory(targetDir);
        File.WriteAllText(_logFilePath, string.Empty);
    }

    public void LogInfo(string message) => WriteLog("INFO", message);
    public void LogSuccess(string message) => WriteLog("SUCCESS", message);
    public void LogError(string message) => WriteLog("ERROR", message);
    public void LogWarning(string message) => WriteLog("WARNING", message);

    private void WriteLog(string level, string message)
    {
        string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}";

        lock (_lockObject)
        {
            File.AppendAllText(_logFilePath, logEntry + Environment.NewLine);
        }

        Console.WriteLine(logEntry);
    }
}