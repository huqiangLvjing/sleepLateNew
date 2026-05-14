using SleepLate.Data.Models;
using SleepLate.Utils;

namespace SleepLate.Services;

class StatisticsService
{
    private static readonly Logger _logger = Logger.Instance;
    private DailyStatistics _todayStats = new();

    public TimeSpan TodayUsageTime => _todayStats.TotalUsageTime;
    public int TodayRestCount => _todayStats.RestCount;

    public void RecordUsage(TimeSpan duration)
    {
        _todayStats.TotalUsageTime += duration;
        _logger.Debug("StatisticsService", $"记录使用时长: {duration.TotalMinutes}分钟");
    }

    public void RecordRest()
    {
        _todayStats.RestCount++;
        _logger.Debug("StatisticsService", $"记录休息次数: {_todayStats.RestCount}");
    }

    public void ResetDaily()
    {
        _todayStats = new DailyStatistics
        {
            Date = DateTime.Today,
            TotalUsageTime = TimeSpan.Zero,
            RestCount = 0
        };
        _logger.Info("StatisticsService", "每日统计已重置");
    }

    public void LoadFromConfig(DateTime lastResetDate)
    {
        if (lastResetDate.Date != DateTime.Today)
        {
            ResetDaily();
        }
    }
}