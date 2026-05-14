using System.Text;
using System.Text.RegularExpressions;

namespace SleepLate.Utils;

class Logger
{
    private static readonly Lazy<Logger> _instance = new(() => new Logger());
    public static Logger Instance => _instance.Value;

    private readonly string _logDir;
    private readonly string _logFile;
    private readonly object _lock = new();

    private Logger()
    {
        _logDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SleepLate", "logs");

        if (!Directory.Exists(_logDir))
        {
            Directory.CreateDirectory(_logDir);
        }

        _logFile = Path.Combine(_logDir, $"sleeplate_{DateTime.Now:yyyyMMdd}.log");
    }

    public void Debug(string module, string message) => Log("DEBUG", module, message);
    public void Info(string module, string message) => Log("INFO ", module, message);
    public void Warn(string module, string message) => Log("WARN ", module, message);
    public void Error(string module, string message) => Log("ERROR", module, message);

    private void Log(string level, string module, string message)
    {
        var logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] [{module}] {message}";

        lock (_lock)
        {
            try
            {
                File.AppendAllText(_logFile, logEntry + Environment.NewLine);
            }
            catch
            {
                // 忽略日志写入失败
            }
        }

        Console.WriteLine(logEntry);
    }
}