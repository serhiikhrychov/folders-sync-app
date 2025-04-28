namespace folders_sync;

public class Logger
{
    private static string _logFilePath;

    public Logger(string logFilePath)
    {
        _logFilePath = logFilePath;
        if (!File.Exists(_logFilePath))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath));
        }
    }

    public void Log(string message)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string logMessageLine = $"[{timestamp}] {message}";
        Console.WriteLine(logMessageLine);
        File.AppendAllText(_logFilePath, logMessageLine + Environment.NewLine);
    }
}