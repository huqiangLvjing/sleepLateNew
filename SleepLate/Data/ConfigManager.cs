using System.Text.Json;
using SleepLate.Data.Models;

namespace SleepLate.Data;

class ConfigManager
{
    private static readonly string ConfigDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "SleepLate");

    private static readonly string ConfigPath = Path.Combine(ConfigDir, "config.json");

    private static readonly Logger _logger = Logger.Instance;

    public AppConfig Load()
    {
        try
        {
            if (!File.Exists(ConfigPath))
            {
                _logger.Info("Config", "配置文件不存在，使用默认配置");
                return CreateDefaultConfig();
            }

            var json = File.ReadAllText(ConfigPath);
            var config = JsonSerializer.Deserialize<AppConfig>(json) ?? CreateDefaultConfig();

            // 检查是否需要重置每日统计
            if (config.LastResetDate.Date != DateTime.Today)
            {
                _logger.Info("Config", "日期变更，重置每日统计");
                config.LastResetDate = DateTime.Today;
            }

            _logger.Info("Config", $"配置加载成功，工作时长: {config.WorkMinutes}分钟，休息时长: {config.RestMinutes}分钟");
            return config;
        }
        catch (Exception ex)
        {
            _logger.Error("Config", $"配置加载失败: {ex.Message}，使用默认配置");
            return CreateDefaultConfig();
        }
    }

    public void Save(AppConfig config)
    {
        try
        {
            if (!Directory.Exists(ConfigDir))
            {
                Directory.CreateDirectory(ConfigDir);
            }

            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);

            _logger.Info("Config", "配置保存成功");
        }
        catch (Exception ex)
        {
            _logger.Error("Config", $"配置保存失败: {ex.Message}");
            throw;
        }
    }

    private AppConfig CreateDefaultConfig()
    {
        return new AppConfig
        {
            WorkMinutes = 45,
            RestMinutes = 5,
            PasswordProtectionEnabled = false,
            TimePeriods = new List<TimePeriod>
            {
                new TimePeriod
                {
                    Name = "工作日",
                    Days = new List<DayOfWeek> { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                    StartTime = new TimeSpan(9, 0, 0),
                    EndTime = new TimeSpan(18, 0, 0),
                    Enabled = true
                },
                new TimePeriod
                {
                    Name = "周末",
                    Days = new List<DayOfWeek> { DayOfWeek.Saturday, DayOfWeek.Sunday },
                    StartTime = new TimeSpan(10, 0, 0),
                    EndTime = new TimeSpan(22, 0, 0),
                    Enabled = true
                }
            }
        };
    }
}