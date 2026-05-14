using SleepLate.Utils;

namespace SleepLate.Services;

class LockScreenService
{
    private static readonly Logger _logger = Logger.Instance;
    private Form? _fullScreenForm;

    public void Lock()
    {
        try
        {
            _logger.Info("LockScreenService", "执行系统锁屏");
            Win32API.LockWorkStation();
        }
        catch (Exception ex)
        {
            _logger.Error("LockScreenService", $"锁屏失败: {ex.Message}");
        }
    }

    public void ShowFullScreenReminder(string message = "休息时间到，请保护眼睛！")
    {
        if (_fullScreenForm != null) return;

        _logger.Info("LockScreenService", "显示全屏提醒");

        _fullScreenForm = new Form
        {
            WindowState = FormWindowState.Maximized,
            FormBorderStyle = FormBorderStyle.None,
            BackColor = System.Drawing.Color.FromArgb(22, 119, 255),
            TopMost = true
        };

        var label = new Label
        {
            Text = message,
            ForeColor = System.Drawing.Color.White,
            Font = new System.Drawing.Font("微软雅黑", 32, System.Drawing.FontStyle.Bold),
            AutoSize = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        };

        var button = new Button
        {
            Text = "我已休息完毕，继续工作",
            FlatStyle = FlatStyle.Flat,
            BackColor = System.Drawing.Color.White,
            ForeColor = System.Drawing.Color.FromArgb(22, 119, 255),
            Font = new System.Drawing.Font("微软雅黑", 16),
            Size = new System.Drawing.Size(300, 60),
            Cursor = Cursors.Hand
        };

        button.Click += (s, e) => HideFullScreenReminder();

        _fullScreenForm.Controls.Add(label);
        _fullScreenForm.Controls.Add(button);

        label.Location = new System.Drawing.Point(
            (_fullScreenForm.Width - label.Width) / 2,
            (_fullScreenForm.Height - label.Height - 80) / 2);

        button.Location = new System.Drawing.Point(
            (_fullScreenForm.Width - button.Width) / 2,
            label.Bottom + 50);

        _fullScreenForm.Show();
    }

    public void HideFullScreenReminder()
    {
        if (_fullScreenForm == null) return;

        _logger.Info("LockScreenService", "关闭全屏提醒");

        _fullScreenForm.Close();
        _fullScreenForm.Dispose();
        _fullScreenForm = null;
    }

    public bool IsFullScreenShowing => _fullScreenForm != null;
}