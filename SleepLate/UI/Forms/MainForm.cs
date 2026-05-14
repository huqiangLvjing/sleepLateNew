using System.Drawing;
using System.Windows.Forms;
using SleepLate.App;
using SleepLate.Services;

namespace SleepLate.UI.Forms;

public class MainForm : Form
{
    private readonly AppContext _ctx = AppContext.Instance;
    private readonly Logger _logger = Logger.Instance;

    // Top navigation
    private Panel _navPanel = null!;
    private Label _titleLabel = null!;
    private Label _subtitleLabel = null!;
    private Label _statusLabel = null!;

    // Timer card
    private Panel _timerCard = null!;
    private Label _timerTitle = null!;
    private TextBox _hoursInput = null!;
    private TextBox _minutesInput = null!;
    private TextBox _secondsInput = null!;
    private Label _usageLabel = null!;
    private Label _restCountLabel = null!;

    // Settings cards
    private Panel _leftCard = null!;
    private Panel _rightCard = null!;

    // Notice bar
    private Panel _noticeBar = null!;

    // Bottom
    private Label _versionLabel = null!;
    private Button _minimizeBtn = null!;
    private Button _exitBtn = null!;

    public MainForm()
    {
        InitializeComponent();
        LoadConfig();
        WireEvents();
    }

    private void InitializeComponent()
    {
        // Form settings
        Text = "护眼小卫士";
        Size = new Size(800, 700);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 247, 250);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        // Global font
        var defaultFont = new Font("微软雅黑", 10F);

        // === Top Navigation ===
        _navPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(22, 119, 255)
        };

        // Icon
        var iconLabel = new Label
        {
            Text = "🕐",
            Font = new Font(defaultFont.FontFamily, 20F),
            Location = new Point(20, 15),
            Size = new Size(35, 35)
        };

        // Title
        _titleLabel = new Label
        {
            Text = "护眼小卫士",
            Font = new Font(defaultFont.FontFamily, 16F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(60, 10),
            Size = new Size(120, 25)
        };

        // Subtitle
        _subtitleLabel = new Label
        {
            Text = "科学用眼 · 定时休息",
            Font = new Font(defaultFont.FontFamily, 9F),
            ForeColor = Color.FromArgb(200, 220, 255),
            Location = new Point(60, 35),
            Size = new Size(150, 20)
        };

        // Status
        _statusLabel = new Label
        {
            Text = "● 运行中",
            Font = new Font(defaultFont.FontFamily, 10F),
            ForeColor = Color.FromArgb(82, 196, 26),
            BackColor = Color.FromArgb(30, 100, 30),
            Location = new Point(620, 18),
            Size = new Size(100, 28),
            TextAlign = ContentAlignment.MiddleCenter
        };

        _navPanel.Controls.AddRange(new Control[] { iconLabel, _titleLabel, _subtitleLabel, _statusLabel });

        // === Timer Card ===
        _timerCard = new Panel
        {
            Location = new Point(50, 80),
            Size = new Size(700, 180),
            BackColor = Color.FromArgb(230, 240, 255),
            BorderStyle = BorderStyle.None
        };

        _timerTitle = new Label
        {
            Text = "距离下次休息还有",
            Font = new Font(defaultFont.FontFamily, 14F),
            ForeColor = Color.FromArgb(31, 35, 41),
            Location = new Point(250, 15),
            Size = new Size(200, 30)
        };

        // Time inputs
        _hoursInput = CreateTimeInput(180, 60, "00");
        _minutesInput = CreateTimeInput(330, 60, "00");
        _secondsInput = CreateTimeInput(480, 60, "00");

        // Colon separators
        var colon1 = new Label { Text = ":", Font = new Font(defaultFont.FontFamily, 28F), Location = new Point(280, 55), Size = new Size(30, 50) };
        var colon2 = new Label { Text = ":", Font = new Font(defaultFont.FontFamily, 28F), Location = new Point(430, 55), Size = new Size(30, 50) };

        // Unit labels
        var hourUnit = new Label { Text = "小时", Font = new Font(defaultFont.FontFamily, 9F), Location = new Point(180, 125), Size = new Size(50, 20), TextAlign = ContentAlignment.TopCenter };
        var minUnit = new Label { Text = "分钟", Font = new Font(defaultFont.FontFamily, 9F), Location = new Point(330, 125), Size = new Size(50, 20), TextAlign = ContentAlignment.TopCenter };
        var secUnit = new Label { Text = "秒", Font = new Font(defaultFont.FontFamily, 9F), Location = new Point(480, 125), Size = new Size(50, 20), TextAlign = ContentAlignment.TopCenter };

        _timerCard.Controls.AddRange(new Control[] { _timerTitle, _hoursInput, _minutesInput, _secondsInput, colon1, colon2, hourUnit, minUnit, secUnit });

        // Statistics
        _usageLabel = new Label
        {
            Text = "今日已使用电脑: 0小时0分钟",
            Font = new Font(defaultFont.FontFamily, 10F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(150, 150),
            Size = new Size(200, 20)
        };

        _restCountLabel = new Label
        {
            Text = "今日已休息: 0次",
            Font = new Font(defaultFont.FontFamily, 10F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(450, 150),
            Size = new Size(150, 20)
        };

        _timerCard.Controls.AddRange(new Control[] { _usageLabel, _restCountLabel });

        // === Settings Cards ===
        _leftCard = CreateSettingsCard(50, 280, "📅", "时间段设置", "设置工作与休息的时间段", false);
        _rightCard = CreateSettingsCard(400, 280, "⚙️", "高级设置", "密码管理与系统配置", true);

        // === Notice Bar ===
        _noticeBar = new Panel
        {
            Location = new Point(50, 390),
            Size = new Size(700, 60),
            BackColor = Color.FromArgb(255, 251, 230),
            BorderStyle = BorderStyle.FixedSingle,
            BorderColor = Color.FromArgb(250, 173, 20)
        };

        var noticeIcon = new Label
        {
            Text = "⚠️",
            Font = new Font(defaultFont.FontFamily, 16F),
            Location = new Point(15, 15),
            Size = new Size(30, 30)
        };

        var noticeText = new Label
        {
            Text = "本程序已启用防退出保护，退出程序需要输入管理员密码。请牢记设置的密码，超级密码可用于紧急解锁。",
            Font = new Font(defaultFont.FontFamily, 9F),
            ForeColor = Color.FromArgb(31, 35, 41),
            Location = new Point(50, 15),
            Size = new Size(630, 35)
        };

        _noticeBar.Controls.AddRange(new Control[] { noticeIcon, noticeText });

        // === Bottom ===
        _versionLabel = new Label
        {
            Text = "SleepLate v1.0.0",
            Font = new Font(defaultFont.FontFamily, 9F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(20, 470),
            Size = new Size(120, 20)
        };

        _minimizeBtn = new Button
        {
            Text = "_",
            Font = new Font(defaultFont.FontFamily, 12F),
            Location = new Point(600, 465),
            Size = new Size(50, 35),
            BackColor = Color.FromArgb(134, 144, 156),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        _exitBtn = new Button
        {
            Text = "×",
            Font = new Font(defaultFont.FontFamily, 14F),
            Location = new Point(660, 465),
            Size = new Size(50, 35),
            BackColor = Color.FromArgb(255, 77, 79),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        // === Add all controls ===
        Controls.AddRange(new Control[]
        {
            _navPanel, _timerCard, _leftCard, _rightCard,
            _noticeBar, _versionLabel, _minimizeBtn, _exitBtn
        });
    }

    private TextBox CreateTimeInput(int x, int y, string defaultText)
    {
        return new TextBox
        {
            Text = defaultText,
            Font = new Font("微软雅黑", 28F, FontStyle.Bold),
            TextAlign = HorizontalAlignment.Center,
            Location = new Point(x, y),
            Size = new Size(100, 60),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            ReadOnly = true
        };
    }

    private Panel CreateSettingsCard(int x, int y, string icon, string title, string desc, bool isRight)
    {
        var card = new Panel
        {
            Location = new Point(x, y),
            Size = new Size(320, 90),
            BackColor = Color.White,
            Tag = isRight ? "advanced" : "timeperiod"
        };

        var iconLabel = new Label
        {
            Text = icon,
            Font = new Font("微软雅黑", 18F),
            Location = new Point(15, 15),
            Size = new Size(30, 30)
        };

        var titleLabel = new Label
        {
            Text = title,
            Font = new Font("微软雅黑", 12F, FontStyle.Bold),
            ForeColor = Color.FromArgb(31, 35, 41),
            Location = new Point(50, 12),
            Size = new Size(100, 25)
        };

        var descLabel = new Label
        {
            Text = desc,
            Font = new Font("微软雅黑", 9F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(50, 35),
            Size = new Size(220, 20)
        };

        var arrow = new Label
        {
            Text = "→",
            Font = new Font("微软雅黑", 14F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(280, 30),
            Size = new Size(30, 30),
            Cursor = Cursors.Hand
        };

        card.Controls.AddRange(new Control[] { iconLabel, titleLabel, descLabel, arrow });

        // Click handlers
        card.Cursor = Cursors.Hand;
        card.Click += (s, e) =>
        {
            if (card.Tag?.ToString() == "timeperiod")
            {
                var form = new TimePeriodForm();
                form.ShowDialog();
            }
            else
            {
                var form = new AdvancedSettingsForm();
                form.ShowDialog();
            }
        };

        return card;
    }

    private void LoadConfig()
    {
        var config = _ctx.ConfigService;

        // Load time from config
        var workMinutes = config.WorkMinutes;
        _hoursInput.Text = (workMinutes / 60).ToString("D2");
        _minutesInput.Text = (workMinutes % 60).ToString("D2");
        _secondsInput.Text = "00";

        UpdateStatistics();
        _logger.Info("MainForm", "主界面加载完成");
    }

    private void WireEvents()
    {
        _ctx.TimerService.OnTick += (s, remaining) =>
        {
            this.Invoke(() =>
            {
                _hoursInput.Text = ((int)remaining.TotalHours).ToString("D2");
                _minutesInput.Text = remaining.Minutes.ToString("D2");
                _secondsInput.Text = remaining.Seconds.ToString("D2");
            });
        };

        _ctx.TimerService.OnWorkFinished += (s, e) =>
        {
            this.Invoke(() =>
            {
                _ctx.ReminderService.TriggerReminder();
                _ctx.StatisticsService.RecordRest();
                UpdateStatistics();
            });
        };

        _ctx.TimerService.OnRestFinished += (s, e) =>
        {
            this.Invoke(() =>
            {
                UpdateStatistics();
            });
        };

        _exitBtn.Click += (s, e) =>
        {
            if (_ctx.PasswordService.PasswordProtectionEnabled && _ctx.PasswordService.HasAdminPasswordSet())
            {
                var pwdForm = new PasswordForm();
                if (pwdForm.ShowDialog() == DialogResult.OK)
                {
                    _logger.Info("MainForm", "用户退出程序");
                    Application.Exit();
                }
            }
            else
            {
                _logger.Info("MainForm", "用户退出程序");
                Application.Exit();
            }
        };

        _minimizeBtn.Click += (s, e) =>
        {
            this.WindowState = FormWindowState.Minimize;
        };
    }

    private void UpdateStatistics()
    {
        var stats = _ctx.StatisticsService;
        var usage = stats.TodayUsageTime;
        _usageLabel.Text = $"今日已使用电脑: {(int)usage.TotalHours}小时{usage.Minutes}分钟";
        _restCountLabel.Text = $"今日已休息: {stats.TodayRestCount}次";
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _logger.Info("MainForm", "主界面已显示");
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _ctx.TimerService.Stop();
        base.OnFormClosing(e);
    }
}