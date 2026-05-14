using SleepLate.Data.Models;
using SleepLate.Utils;

namespace SleepLate.Services;

class ReminderService
{
    private static readonly Logger _logger = Logger.Instance;
    private readonly LockScreenService _lockScreenService;

    public ReminderMode CurrentMode { get; set; } = ReminderMode.FullScreenLock;
    public bool SoundEnabled { get; set; } = true;

    public ReminderService(LockScreenService lockScreenService)
    {
        _lockScreenService = lockScreenService;
    }

    public void TriggerReminder()
    {
        _logger.Info("ReminderService", $"触发提醒，模式: {CurrentMode}");

        switch (CurrentMode)
        {
            case ReminderMode.FullScreenLock:
                _lockScreenService.ShowFullScreenReminder("休息时间到，请保护眼睛！");
                break;

            case ReminderMode.Popup:
                ShowPopupReminder();
                break;

            case ReminderMode.Silent:
                _logger.Info("ReminderService", "静默模式，仅记录");
                break;
        }

        if (SoundEnabled)
        {
            PlaySound();
        }
    }

    private void ShowPopupReminder()
    {
        var form = new Form
        {
            Width = 400,
            Height = 200,
            StartPosition = FormStartPosition.CenterScreen,
            TopMost = true,
            BackColor = System.Drawing.Color.White,
            Text = "护眼提醒"
        };

        var label = new Label
        {
            Text = "休息时间到！\n请让眼睛休息一下。",
            Font = new System.Drawing.Font("微软雅黑", 16),
            AutoSize = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        };

        var button = new Button
        {
            Text = "继续工作",
            DialogResult = DialogResult.OK
        };

        label.SetBounds(50, 30, 300, 60);
        button.SetBounds(150, 110, 100, 35);

        form.Controls.Add(label);
        form.Controls.Add(button);

        _logger.Info("ReminderService", "显示弹窗提醒");

        form.ShowDialog();
    }

    private void PlaySound()
    {
        try
        {
            System.Media.SystemSounds.Asterisk.Play();
            _logger.Debug("ReminderService", "播放提示音");
        }
        catch (Exception ex)
        {
            _logger.Error("ReminderService", $"播放提示音失败: {ex.Message}");
        }
    }
}