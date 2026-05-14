using SleepLate.Data;
using SleepLate.Data.Models;
using SleepLate.Utils;

namespace SleepLate.Services;

enum TimerState
{
    Stopped,
    Running,
    Paused,
    Resting
}

class TimerService
{
    private static readonly Logger _logger = Logger.Instance;
    private readonly ConfigManager _configManager;
    private System.Timers.Timer? _timer;
    private TimerState _state = TimerState.Stopped;
    private TimeSpan _remainingTime;
    private int _workMinutes;
    private int _restMinutes;
    private bool _isInWorkPeriod = true;

    public TimerService(ConfigManager configManager)
    {
        _configManager = configManager;
    }

    public TimeSpan RemainingTime => _remainingTime;
    public TimerState State => _state;
    public bool IsInWorkPeriod => _isInWorkPeriod;

    public event EventHandler<TimeSpan>? OnTick;
    public event EventHandler? OnWorkFinished;
    public event EventHandler? OnRestFinished;
    public event EventHandler? OnDayReset;

    public void Start(int workMinutes)
    {
        _workMinutes = workMinutes;
        _restMinutes = _configManager.Load().RestMinutes;
        _remainingTime = TimeSpan.FromMinutes(_workMinutes);
        _isInWorkPeriod = true;
        _state = TimerState.Running;

        StartTimer();

        _logger.Info("TimerService", $"定时器已启动，工作时长: {workMinutes}分钟");
    }

    public void Pause()
    {
        if (_state != TimerState.Running) return;

        _timer?.Stop();
        _state = TimerState.Paused;

        _logger.Info("TimerService", "定时器已暂停");
    }

    public void Resume()
    {
        if (_state != TimerState.Paused) return;

        _timer?.Start();
        _state = TimerState.Running;

        _logger.Info("TimerService", "定时器已恢复");
    }

    public void Stop()
    {
        _timer?.Stop();
        _timer?.Dispose();
        _timer = null;
        _remainingTime = TimeSpan.Zero;
        _state = TimerState.Stopped;

        _logger.Info("TimerService", "定时器已停止");
    }

    private void StartTimer()
    {
        _timer?.Stop();
        _timer?.Dispose();

        _timer = new System.Timers.Timer(1000);
        _timer.Elapsed += (s, e) =>
        {
            if (_state != TimerState.Running && _state != TimerState.Resting) return;

            _remainingTime = _remainingTime.Subtract(TimeSpan.FromSeconds(1));

            if (_remainingTime <= TimeSpan.Zero)
            {
                _remainingTime = TimeSpan.Zero;
                OnTick?.Invoke(this, _remainingTime);

                if (_state == TimerState.Running)
                {
                    _logger.Info("TimerService", "工作时间到，触发休息提醒");
                    _isInWorkPeriod = false;
                    OnWorkFinished?.Invoke(this, EventArgs.Empty);
                    StartRestPeriod();
                }
                else if (_state == TimerState.Resting)
                {
                    _logger.Info("TimerService", "休息时间到");
                    _isInWorkPeriod = true;
                    OnRestFinished?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                OnTick?.Invoke(this, _remainingTime);
            }
        };

        _timer.Start();
    }

    private void StartRestPeriod()
    {
        _remainingTime = TimeSpan.FromMinutes(_restMinutes);
        _state = TimerState.Resting;
        _logger.Info("TimerService", $"开始休息，时长: {_restMinutes}分钟");
    }

    public void CheckDayReset()
    {
        if (DateTime.Today > _configManager.Load().LastResetDate)
        {
            _logger.Info("TimerService", "检测到日期变更，重置统计");
            OnDayReset?.Invoke(this, EventArgs.Empty);
        }
    }
}