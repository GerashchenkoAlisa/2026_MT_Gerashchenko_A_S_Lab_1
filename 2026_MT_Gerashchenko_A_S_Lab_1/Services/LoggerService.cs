using System.IO;

public class LoggerService : ILoggerService
{
    private readonly string _logFilePath;
    private readonly object _lockObject = new object();

    public LoggerService(string targetDir)
    {
        if (string.IsNullOrWhiteSpace(targetDir))
            throw new ArgumentException("Target directory cannot be null or empty.", nameof(targetDir));

        Directory.CreateDirectory(targetDir);

        string logFileName = GenerateLogFileName(targetDir);
        string parentDir = Path.GetDirectoryName(targetDir) ?? Directory.GetCurrentDirectory();
        _logFilePath = Path.Combine(parentDir, logFileName);
    }

    public static string GenerateLogFileName(string targetDir)
    {
        string workDirName = Path.GetFileName(targetDir.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
        if (string.IsNullOrEmpty(workDirName))
            workDirName = "unknown";

        string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
        return $"CICD_{workDirName}_{timestamp}.log";
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