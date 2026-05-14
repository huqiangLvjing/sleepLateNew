using System.Drawing;
using System.Windows.Forms;
using SleepLate.App;
using SleepLate.Data.Models;
using SleepLate.Utils;

namespace SleepLate.UI.Forms;

public class TimePeriodForm : Form
{
    private readonly AppContext _ctx = AppContext.Instance;
    private readonly Logger _logger = Logger.Instance;

    private NumericUpDown _workMinutesInput = null!;
    private NumericUpDown _restMinutesInput = null!;
    private RadioButton _fullScreenRadio = null!;
    private RadioButton _popupRadio = null!;
    private RadioButton _silentRadio = null!;
    private CheckBox _soundCheckBox = null!;
    private ListView _periodListView = null!;

    public TimePeriodForm()
    {
        InitializeComponent();
        LoadConfig();
    }

    private void InitializeComponent()
    {
        Text = "工作时间设置";
        Size = new Size(500, 600);
        StartPosition = FormStartPosition.CenterScreen;
        BackColor = Color.FromArgb(245, 247, 250);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;

        var defaultFont = new Font("微软雅黑", 10F);

        // Top navigation
        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            BackColor = Color.FromArgb(22, 119, 255)
        };

        var backBtn = new Button
        {
            Text = "←",
            Font = new Font(defaultFont.FontFamily, 16F),
            Location = new Point(15, 15),
            Size = new Size(40, 35),
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Flat,
            ForeColor = Color.White,
            Cursor = Cursors.Hand
        };
        backBtn.Click += (s, e) => Close();

        var titleLabel = new Label
        {
            Text = "工作时间设置",
            Font = new Font(defaultFont.FontFamily, 14F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(65, 10),
            Size = new Size(150, 25)
        };

        var subtitleLabel = new Label
        {
            Text = "设置提醒时间与生效时段",
            Font = new Font(defaultFont.FontFamily, 9F),
            ForeColor = Color.FromArgb(200, 220, 255),
            Location = new Point(65, 35),
            Size = new Size(180, 20)
        };

        topPanel.Controls.AddRange(new Control[] { backBtn, titleLabel, subtitleLabel });

        // Notice card
        var noticeCard = new Panel
        {
            Location = new Point(20, 75),
            Size = new Size(460, 50),
            BackColor = Color.White,
            Padding = new Padding(10)
        };

        var bulbIcon = new Label
        {
            Text = "💡",
            Font = new Font(defaultFont.FontFamily, 16F),
            Location = new Point(10, 8),
            Size = new Size(30, 30)
        };

        var noticeText = new Label
        {
            Text = "仅在设置时间段内休息提醒生效",
            Font = new Font(defaultFont.FontFamily, 10F),
            ForeColor = Color.FromArgb(31, 35, 41),
            Location = new Point(45, 12),
            Size = new Size(400, 25)
        };

        noticeCard.Controls.AddRange(new Control[] { bulbIcon, noticeText });

        // Work/Rest minutes setting
        var minutesCard = new Panel
        {
            Location = new Point(20, 140),
            Size = new Size(460, 100),
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        var workLabel = new Label
        {
            Text = "连续工作时长",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(15, 15),
            Size = new Size(100, 25)
        };

        _workMinutesInput = new NumericUpDown
        {
            Location = new Point(15, 40),
            Size = new Size(80, 30),
            Value = 45,
            Minimum = 1,
            Maximum = 480,
            Increment = 5
        };

        var workUnit = new Label
        {
            Text = "分钟",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(100, 40),
            Size = new Size(40, 30)
        };

        var workHint = new Label
        {
            Text = "建议工作45分钟后休息一次",
            Font = new Font(defaultFont.FontFamily, 8F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(15, 72),
            Size = new Size(200, 20)
        };

        var restLabel = new Label
        {
            Text = "每次休息时长",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(250, 15),
            Size = new Size(100, 25)
        };

        _restMinutesInput = new NumericUpDown
        {
            Location = new Point(250, 40),
            Size = new Size(80, 30),
            Value = 5,
            Minimum = 1,
            Maximum = 60,
            Increment = 1
        };

        var restUnit = new Label
        {
            Text = "分钟",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(335, 40),
            Size = new Size(40, 30)
        };

        var restHint = new Label
        {
            Text = "休息5分钟保护眼睛健康",
            Font = new Font(defaultFont.FontFamily, 8F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(250, 72),
            Size = new Size(200, 20)
        };

        minutesCard.Controls.AddRange(new Control[] {
            workLabel, _workMinutesInput, workUnit, workHint,
            restLabel, _restMinutesInput, restUnit, restHint
        });

        // Reminder mode
        var reminderCard = new Panel
        {
            Location = new Point(20, 255),
            Size = new Size(460, 130),
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        var reminderTitle = new Label
        {
            Text = "提醒方式",
            Font = new Font(defaultFont.FontFamily, 11F, FontStyle.Bold),
            Location = new Point(15, 10),
            Size = new Size(100, 25)
        };

        _fullScreenRadio = new RadioButton
        {
            Text = "全屏锁定提醒 - 强制锁屏保护眼睛",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(15, 40),
            Size = new Size(280, 25),
            Checked = true
        };

        _popupRadio = new RadioButton
        {
            Text = "弹窗提醒 - 提示后继续工作",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(15, 65),
            Size = new Size(220, 25)
        };

        _silentRadio = new RadioButton
        {
            Text = "静默提醒 - 只记录不打扰",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(15, 90),
            Size = new Size(220, 25)
        };

        _soundCheckBox = new CheckBox
        {
            Text = "提醒音效开启",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(280, 40),
            Size = new Size(150, 25),
            Checked = true
        };

        reminderCard.Controls.AddRange(new Control[] {
            reminderTitle, _fullScreenRadio, _popupRadio, _silentRadio, _soundCheckBox
        });

        // Bottom buttons
        var cancelBtn = new Button
        {
            Text = "取消",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(100, 520),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(245, 247, 250),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        var saveBtn = new Button
        {
            Text = "保存设置",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(260, 520),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(22, 119, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        cancelBtn.Click += (s, e) => Close();

        saveBtn.Click += (s, e) =>
        {
            SaveConfig();
            DialogResult = DialogResult.OK;
            Close();
        };

        Controls.AddRange(new Control[]
        {
            topPanel, noticeCard, minutesCard, reminderCard,
            cancelBtn, saveBtn
        });
    }

    private void LoadConfig()
    {
        var config = _ctx.ConfigService;
        _workMinutesInput.Value = config.WorkMinutes;
        _restMinutesInput.Value = config.RestMinutes;
        _soundCheckBox.Checked = config.SoundEnabled;

        switch (config.ReminderMode)
        {
            case ReminderMode.FullScreenLock:
                _fullScreenRadio.Checked = true;
                break;
            case ReminderMode.Popup:
                _popupRadio.Checked = true;
                break;
            case ReminderMode.Silent:
                _silentRadio.Checked = true;
                break;
        }

        _logger.Debug("TimePeriodForm", "配置已加载");
    }

    private void SaveConfig()
    {
        var config = _ctx.ConfigService;
        config.WorkMinutes = (int)_workMinutesInput.Value;
        config.RestMinutes = (int)_restMinutesInput.Value;
        config.SoundEnabled = _soundCheckBox.Checked;

        if (_fullScreenRadio.Checked)
            config.ReminderMode = ReminderMode.FullScreenLock;
        else if (_popupRadio.Checked)
            config.ReminderMode = ReminderMode.Popup;
        else
            config.ReminderMode = ReminderMode.Silent;

        config.Save();

        // Sync to services
        _ctx.ReminderService.CurrentMode = config.ReminderMode;
        _ctx.ReminderService.SoundEnabled = config.SoundEnabled;

        _logger.Info("TimePeriodForm", "配置已保存");
    }
}