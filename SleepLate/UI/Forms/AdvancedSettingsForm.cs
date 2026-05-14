using System.Drawing;
using System.Windows.Forms;
using SleepLate.App;
using SleepLate.Utils;

namespace SleepLate.UI.Forms;

public class AdvancedSettingsForm : Form
{
    private readonly AppContext _ctx = AppContext.Instance;
    private readonly Logger _logger = Logger.Instance;

    private TextBox _adminPwdInput = null!;
    private TextBox _superPwdInput = null!;
    private CheckBox _pwdProtectionCheck = null!;
    private CheckBox _autoStartCheck = null!;

    public AdvancedSettingsForm()
    {
        InitializeComponent();
        LoadConfig();
    }

    private void InitializeComponent()
    {
        Text = "高级设置";
        Size = new Size(500, 550);
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
            Text = "高级设置",
            Font = new Font(defaultFont.FontFamily, 14F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(65, 10),
            Size = new Size(100, 25)
        };

        var subtitleLabel = new Label
        {
            Text = "密码与系统配置",
            Font = new Font(defaultFont.FontFamily, 9F),
            ForeColor = Color.FromArgb(200, 220, 255),
            Location = new Point(65, 35),
            Size = new Size(120, 20)
        };

        topPanel.Controls.AddRange(new Control[] { backBtn, titleLabel, subtitleLabel });

        // Warning bar
        var warningBar = new Panel
        {
            Location = new Point(20, 75),
            Size = new Size(460, 45),
            BackColor = Color.FromArgb(255, 240, 240)
        };

        var warningIcon = new Label
        {
            Text = "⚠️",
            Font = new Font(defaultFont.FontFamily, 14F),
            Location = new Point(15, 8),
            Size = new Size(25, 25)
        };

        var warningText = new Label
        {
            Text = "请妥善保管管理员密码与超级密码",
            Font = new Font(defaultFont.FontFamily, 10F),
            ForeColor = Color.FromArgb(255, 77, 79),
            Location = new Point(45, 10),
            Size = new Size(400, 25)
        };

        warningBar.Controls.AddRange(new Control[] { warningIcon, warningText });

        // Password settings card
        var pwdCard = new Panel
        {
            Location = new Point(20, 135),
            Size = new Size(460, 220),
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        // Admin password row
        var adminLabel = new Label
        {
            Text = "管理员密码",
            Font = new Font(defaultFont.FontFamily, 11F),
            Location = new Point(15, 15),
            Size = new Size(100, 25)
        };

        var adminDesc = new Label
        {
            Text = "用于退出程序验证",
            Font = new Font(defaultFont.FontFamily, 9F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(15, 38),
            Size = new Size(150, 20)
        };

        _adminPwdInput = new TextBox
        {
            PasswordChar = '*',
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(15, 58),
            Size = new Size(200, 30),
            BorderStyle = BorderStyle.FixedSingle
        };

        var adminSetBtn = new Button
        {
            Text = "设置密码",
            Font = new Font(defaultFont.FontFamily, 9F),
            Location = new Point(230, 58),
            Size = new Size(80, 30),
            BackColor = Color.FromArgb(22, 119, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        // Super password row
        var superLabel = new Label
        {
            Text = "超级后门密码",
            Font = new Font(defaultFont.FontFamily, 11F),
            Location = new Point(15, 100),
            Size = new Size(110, 25)
        };

        var superDesc = new Label
        {
            Text = "紧急解锁用",
            Font = new Font(defaultFont.FontFamily, 9F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(15, 123),
            Size = new Size(150, 20)
        };

        _superPwdInput = new TextBox
        {
            PasswordChar = '*',
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(15, 143),
            Size = new Size(200, 30),
            BorderStyle = BorderStyle.FixedSingle
        };

        var superSetBtn = new Button
        {
            Text = "设置密码",
            Font = new Font(defaultFont.FontFamily, 9F),
            Location = new Point(230, 143),
            Size = new Size(80, 30),
            BackColor = Color.FromArgb(22, 119, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        adminSetBtn.Click += (s, e) => SetAdminPassword();
        superSetBtn.Click += (s, e) => SetSuperPassword();

        // Password protection toggle
        _pwdProtectionCheck = new CheckBox
        {
            Text = "退出密码保护",
            Font = new Font(defaultFont.FontFamily, 11F),
            Location = new Point(15, 185),
            Size = new Size(150, 25)
        };
        _pwdProtectionCheck.CheckedChanged += (s, e) =>
        {
            _ctx.ConfigService.PasswordProtectionEnabled = _pwdProtectionCheck.Checked;
            _ctx.PasswordService.PasswordProtectionEnabled = _pwdProtectionCheck.Checked;
        };

        // Auto start toggle
        _autoStartCheck = new CheckBox
        {
            Text = "开机自动启动",
            Font = new Font(defaultFont.FontFamily, 11F),
            Location = new Point(230, 185),
            Size = new Size(150, 25)
        };
        _autoStartCheck.CheckedChanged += (s, e) =>
        {
            _ctx.ConfigService.AutoStartEnabled = _autoStartCheck.Checked;
            _ctx.AutoStartService.SetEnabled(_autoStartCheck.Checked);
        };

        pwdCard.Controls.AddRange(new Control[] {
            adminLabel, adminDesc, _adminPwdInput, adminSetBtn,
            superLabel, superDesc, _superPwdInput, superSetBtn,
            _pwdProtectionCheck, _autoStartCheck
        });

        // About card
        var aboutCard = new Panel
        {
            Location = new Point(20, 370),
            Size = new Size(460, 80),
            BackColor = Color.White,
            Padding = new Padding(15)
        };

        var aboutTitle = new Label
        {
            Text = "护眼小卫士 v1.0.0",
            Font = new Font(defaultFont.FontFamily, 12F, FontStyle.Bold),
            Location = new Point(15, 15),
            Size = new Size(200, 25)
        };

        var aboutDesc = new Label
        {
            Text = "科学用眼，定时休息，保护视力健康",
            Font = new Font(defaultFont.FontFamily, 10F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(15, 45),
            Size = new Size(300, 25)
        };

        aboutCard.Controls.AddRange(new Control[] { aboutTitle, aboutDesc });

        // Bottom buttons
        var cancelBtn = new Button
        {
            Text = "取消",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(100, 470),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(245, 247, 250),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        var saveBtn = new Button
        {
            Text = "保存设置",
            Font = new Font(defaultFont.FontFamily, 10F),
            Location = new Point(260, 470),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(22, 119, 255),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        cancelBtn.Click += (s, e) => Close();

        saveBtn.Click += (s, e) =>
        {
            _ctx.ConfigService.Save();
            _logger.Info("AdvancedSettingsForm", "高级设置已保存");
            DialogResult = DialogResult.OK;
            Close();
        };

        Controls.AddRange(new Control[]
        {
            topPanel, warningBar, pwdCard, aboutCard,
            cancelBtn, saveBtn
        });
    }

    private void LoadConfig()
    {
        var config = _ctx.ConfigService;
        _pwdProtectionCheck.Checked = config.PasswordProtectionEnabled;
        _autoStartCheck.Checked = config.AutoStartEnabled;
        _logger.Debug("AdvancedSettingsForm", "配置已加载");
    }

    private void SetAdminPassword()
    {
        var pwd = _adminPwdInput.Text;
        if (string.IsNullOrEmpty(pwd) || pwd.Length < 6)
        {
            MessageBox.Show("密码长度至少6位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _ctx.PasswordService.SetAdminPassword(pwd);
        _ctx.ConfigService.EncryptedAdminPassword = _ctx.ConfigService.EncryptedAdminPassword; // Trigger save

        var config = _ctx.ConfigService;
        var encrypted = EncryptUtil.Encrypt(pwd);
        config.EncryptedAdminPassword = encrypted;
        config.Save();

        _adminPwdInput.Clear();
        MessageBox.Show("管理员密码已设置", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        _logger.Info("AdvancedSettingsForm", "管理员密码已设置");
    }

    private void SetSuperPassword()
    {
        var pwd = _superPwdInput.Text;
        if (string.IsNullOrEmpty(pwd) || pwd.Length < 6)
        {
            MessageBox.Show("密码长度至少6位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var config = _ctx.ConfigService;
        var encrypted = EncryptUtil.Encrypt(pwd);
        config.EncryptedSuperPassword = encrypted;
        config.Save();

        _ctx.PasswordService.SetSuperPassword(pwd);

        _superPwdInput.Clear();
        MessageBox.Show("超级密码已设置", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        _logger.Info("AdvancedSettingsForm", "超级密码已设置");
    }
}