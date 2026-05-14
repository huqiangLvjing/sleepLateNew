using SleepLate.Data;
using SleepLate.Data.Models;
using SleepLate.Utils;

namespace SleepLate.Services;

class ConfigService
{
    private static readonly Logger _logger = Logger.Instance;
    private readonly ConfigManager _configManager;
    private AppConfig _config;

    public ConfigService(ConfigManager configManager)
    {
        _configManager = configManager;
        _config = _configManager.Load();
    }

    public void Reload()
    {
        _config = _configManager.Load();
        _logger.Info("ConfigService", "配置已重新加载");
    }

    public void Save()
    {
        _configManager.Save(_config);
    }

    public int WorkMinutes
    {
        get => _config.WorkMinutes;
        set
        {
            _config.WorkMinutes = value;
            _logger.Debug("ConfigService", $"工作时长设置为{value}分钟");
        }
    }

    public int RestMinutes
    {
        get => _config.RestMinutes;
        set
        {
            _config.RestMinutes = value;
            _logger.Debug("ConfigService", $"休息时长设置为{value}分钟");
        }
    }

    public ReminderMode ReminderMode
    {
        get => _config.ReminderMode;
        set
        {
            _config.ReminderMode = value;
            _logger.Debug("ConfigService", $"提醒模式设置为{value}");
        }
    }

    public bool SoundEnabled
    {
        get => _config.SoundEnabled;
        set
        {
            _config.SoundEnabled = value;
            _logger.Debug("ConfigService", $"提醒音效设置为{value}");
        }
    }

    public List<TimePeriod> TimePeriods => _config.TimePeriods;

    public string EncryptedAdminPassword
    {
        get => _config.EncryptedAdminPassword;
        set => _config.EncryptedAdminPassword = value;
    }

    public string EncryptedSuperPassword
    {
        get => _config.EncryptedSuperPassword;
        set => _config.EncryptedSuperPassword = value;
    }

    public bool PasswordProtectionEnabled
    {
        get => _config.PasswordProtectionEnabled;
        set => _config.PasswordProtectionEnabled = value;
    }

    public bool AutoStartEnabled
    {
        get => _config.AutoStartEnabled;
        set => _config.AutoStartEnabled = value;
    }

    public bool IsInActivePeriod()
    {
        foreach (var period in _config.TimePeriods)
        {
            if (period.IsNowInPeriod())
            {
                return true;
            }
        }
        return false;
    }
}