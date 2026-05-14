using System.Drawing;
using System.Windows.Forms;
using SleepLate.App;
using SleepLate.Utils;

namespace SleepLate.UI.Forms;

public class PasswordForm : Form
{
    private readonly AppContext _ctx = AppContext.Instance;
    private readonly Logger _logger = Logger.Instance;

    private TextBox _adminPwdInput = null!;
    private TextBox _superPwdInput = null!;
    private Label _errorLabel = null!;
    private CheckBox _showPwdCheck = null!;

    public PasswordForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        Text = "身份验证";
        Size = new Size(450, 420);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;

        // Top panel - Red gradient
        var topPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 120,
            BackColor = Color.FromArgb(255, 77, 79)
        };

        var lockIcon = new Label
        {
            Text = "🔒",
            Font = new Font("微软雅黑", 32F),
            ForeColor = Color.White,
            Location = new Point(185, 15),
            Size = new Size(60, 50)
        };

        var titleLabel = new Label
        {
            Text = "身份验证",
            Font = new Font("微软雅黑", 18F, FontStyle.Bold),
            ForeColor = Color.White,
            Location = new Point(165, 65),
            Size = new Size(120, 30)
        };

        var subtitleLabel = new Label
        {
            Text = "请输入管理员密码以退出程序",
            Font = new Font("微软雅黑", 9F),
            ForeColor = Color.FromArgb(255, 200, 200),
            Location = new Point(110, 90),
            Size = new Size(230, 20)
        };

        topPanel.Controls.AddRange(new Control[] { lockIcon, titleLabel, subtitleLabel });

        // Warning bar
        var warningBar = new Panel
        {
            Dock = DockStyle.Top,
            Height = 50,
            BackColor = Color.FromArgb(255, 251, 230),
            Padding = new Padding(10)
        };

        var warningIcon = new Label
        {
            Text = "⚠️",
            Font = new Font("微软雅黑", 14F),
            Location = new Point(10, 10),
            Size = new Size(25, 25)
        };

        var warningText = new Label
        {
            Text = "连续3次错误输入将锁定程序，超级密码可用于紧急解锁",
            Font = new Font("微软雅黑", 9F),
            ForeColor = Color.FromArgb(31, 35, 41),
            Location = new Point(40, 10),
            Size = new Size(360, 30)
        };

        warningBar.Controls.AddRange(new Control[] { warningIcon, warningText });

        // Admin password section
        var adminLabel = new Label
        {
            Text = "管理员密码",
            Font = new Font("微软雅黑", 10F),
            Location = new Point(30, 30),
            Size = new Size(100, 25)
        };

        _adminPwdInput = new TextBox
        {
            PasswordChar = '*',
            Font = new Font("微软雅黑", 12F),
            Location = new Point(30, 55),
            Size = new Size(320, 35),
            BorderStyle = BorderStyle.FixedSingle
        };

        _showPwdCheck = new CheckBox
        {
            Text = "显示密码",
            Font = new Font("微软雅黑", 9F),
            Location = new Point(30, 95),
            Size = new Size(100, 20),
            Checked = false
        };
        _showPwdCheck.CheckedChanged += (s, e) =>
        {
            _adminPwdInput.PasswordChar = _showPwdCheck.Checked ? '\0' : '*';
        };

        _errorLabel = new Label
        {
            Text = "",
            Font = new Font("微软雅黑", 9F),
            ForeColor = Color.FromArgb(255, 77, 79),
            Location = new Point(30, 115),
            Size = new Size(320, 20)
        };

        var forgetLabel = new Label
        {
            Text = "忘记密码？使用超级密码",
            Font = new Font("微软雅黑", 9F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(30, 135),
            Size = new Size(180, 20)
        };

        // Super password section
        var superLabel = new Label
        {
            Text = "超级密码（后门）",
            Font = new Font("微软雅黑", 10F),
            Location = new Point(30, 170),
            Size = new Size(130, 25)
        };

        _superPwdInput = new TextBox
        {
            PasswordChar = '*',
            Font = new Font("微软雅黑", 12F),
            Location = new Point(30, 195),
            Size = new Size(320, 35),
            BorderStyle = BorderStyle.FixedSingle
        };

        var superHint = new Label
        {
            Text = "万能密码可无条件解锁程序",
            Font = new Font("微软雅黑", 9F),
            ForeColor = Color.FromArgb(134, 144, 156),
            Location = new Point(30, 235),
            Size = new Size(200, 20)
        };

        // Buttons
        var cancelBtn = new Button
        {
            Text = "取消",
            Font = new Font("微软雅黑", 10F),
            Location = new Point(80, 280),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(245, 247, 250),
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        var confirmBtn = new Button
        {
            Text = "确认退出",
            Font = new Font("微软雅黑", 10F),
            Location = new Point(230, 280),
            Size = new Size(120, 40),
            BackColor = Color.FromArgb(255, 77, 79),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Cursor = Cursors.Hand
        };

        cancelBtn.Click += (s, e) =>
        {
            _logger.Info("PasswordForm", "用户取消退出");
            DialogResult = DialogResult.Cancel;
            Close();
        };

        confirmBtn.Click += (s, e) =>
        {
            VerifyAndExit();
        };

        _adminPwdInput.KeyDown += (s, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                VerifyAndExit();
            }
        };

        Controls.AddRange(new Control[]
        {
            topPanel, warningBar, adminLabel, _adminPwdInput,
            _showPwdCheck, _errorLabel, forgetLabel,
            superLabel, _superPwdInput, superHint,
            cancelBtn, confirmBtn
        });
    }

    private void VerifyAndExit()
    {
        var adminPwd = _adminPwdInput.Text;
        var superPwd = _superPwdInput.Text;

        // Try super password first (backdoor)
        if (!string.IsNullOrEmpty(superPwd))
        {
            if (_ctx.PasswordService.VerifySuperPassword(superPwd))
            {
                _logger.Info("PasswordForm", "超级密码验证成功，退出程序");
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
        }

        // Try admin password
        if (!string.IsNullOrEmpty(adminPwd))
        {
            if (_ctx.PasswordService.VerifyAdminPassword(adminPwd))
            {
                _logger.Info("PasswordForm", "管理员密码验证成功，退出程序");
                DialogResult = DialogResult.OK;
                Close();
                return;
            }
        }

        // Verification failed
        var remaining = _ctx.PasswordService.GetRemainingAttempts();
        _errorLabel.Text = $"密码错误，剩余{remaining}次";
        _logger.Warn("PasswordForm", $"密码验证失败，剩余{remaining}次");

        if (remaining <= 0)
        {
            MessageBox.Show("密码连续错误，程序已锁定。\n请重启程序或使用超级密码解锁。", "锁定提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        _adminPwdInput.Clear();
        _superPwdInput.Clear();
        _adminPwdInput.Focus();
    }
}