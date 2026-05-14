namespace SleepLate.Data.Models;

public enum ReminderMode
{
    FullScreenLock,  // 全屏锁定
    Popup,           // 弹窗提醒
    Silent           // 静默
}

public class AppConfig
{
    public int WorkMinutes { get; set; } = 45;
    public int RestMinutes { get; set; } = 5;
    public string EncryptedAdminPassword { get; set; } = "";
    public string EncryptedSuperPassword { get; set; } = "";
    public bool PasswordProtectionEnabled { get; set; } = false;
    public bool AutoStartEnabled { get; set; } = false;
    public ReminderMode ReminderMode { get; set; } = ReminderMode.FullScreenLock;
    public bool SoundEnabled { get; set; } = true;
    public List<TimePeriod> TimePeriods { get; set; } = new();
    public DateTime LastResetDate { get; set; } = DateTime.Today;
}

public class TimePeriod
{
    public string Name { get; set; } = "";
    public List<DayOfWeek> Days { get; set; } = new();
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool Enabled { get; set; } = true;

    public bool IsNowInPeriod()
    {
        if (!Enabled) return false;

        var now = DateTime.Now;
        var currentDay = now.DayOfWeek;
        var currentTime = now.TimeOfDay;

        if (!Days.Contains(currentDay)) return false;

        return currentTime >= StartTime && currentTime <= EndTime;
    }
}

public class DailyStatistics
{
    public DateTime Date { get; set; } = DateTime.Today;
    public TimeSpan TotalUsageTime { get; set; } = TimeSpan.Zero;
    public int RestCount { get; set; } = 0;
}