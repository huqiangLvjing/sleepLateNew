using SleepLate.Utils;

namespace SleepLate.Services;

class PasswordService
{
    private static readonly Logger _logger = Logger.Instance;

    private const int MaxFailedAttempts = 3;
    private string _encryptedAdminPassword = "";
    private string _encryptedSuperPassword = "";
    private int _failedAttempts = 0;

    public bool PasswordProtectionEnabled { get; set; } = false;
    public bool IsLocked { get; private set; } = false;

    public void SetAdminPassword(string password)
    {
        _encryptedAdminPassword = EncryptUtil.Encrypt(password);
        _logger.Info("PasswordService", "管理员密码已设置");
    }

    public void SetSuperPassword(string password)
    {
        _encryptedSuperPassword = EncryptUtil.Encrypt(password);
        _logger.Info("PasswordService", "超级密码已设置");
    }

    public void LoadPasswords(string encryptedAdmin, string encryptedSuper, bool protectionEnabled)
    {
        _encryptedAdminPassword = encryptedAdmin;
        _encryptedSuperPassword = encryptedSuper;
        PasswordProtectionEnabled = protectionEnabled;
        _failedAttempts = 0;
        IsLocked = false;
    }

    public bool VerifyAdminPassword(string password)
    {
        if (IsLocked)
        {
            _logger.Warn("PasswordService", "密码验证被锁定");
            return false;
        }

        if (string.IsNullOrEmpty(_encryptedAdminPassword))
        {
            _logger.Warn("PasswordService", "管理员密码未设置");
            return false;
        }

        var decrypted = EncryptUtil.Decrypt(_encryptedAdminPassword);
        var success = decrypted == password;

        if (success)
        {
            _failedAttempts = 0;
            _logger.Info("PasswordService", "管理员密码验证成功");
        }
        else
        {
            _failedAttempts++;
            var remaining = MaxFailedAttempts - _failedAttempts;
            _logger.Warn("PasswordService", $"管理员密码验证失败，剩余{remaining}次");

            if (_failedAttempts >= MaxFailedAttempts)
            {
                IsLocked = true;
                _logger.Error("PasswordService", "密码验证锁定，请使用超级密码或重启程序");
            }
        }

        return success;
    }

    public bool VerifySuperPassword(string password)
    {
        if (string.IsNullOrEmpty(_encryptedSuperPassword))
        {
            _logger.Warn("PasswordService", "超级密码未设置");
            return false;
        }

        var decrypted = EncryptUtil.Decrypt(_encryptedSuperPassword);
        var success = decrypted == password;

        if (success)
        {
            _failedAttempts = 0;
            IsLocked = false;
            _logger.Info("PasswordService", "超级密码验证成功，可解锁");
        }
        else
        {
            _logger.Warn("PasswordService", "超级密码验证失败");
        }

        return success;
    }

    public int GetRemainingAttempts()
    {
        return MaxFailedAttempts - _failedAttempts;
    }

    public void Reset()
    {
        _failedAttempts = 0;
        IsLocked = false;
    }

    public bool HasAdminPasswordSet()
    {
        return !string.IsNullOrEmpty(_encryptedAdminPassword);
    }

    public bool HasSuperPasswordSet()
    {
        return !string.IsNullOrEmpty(_encryptedSuperPassword);
    }
}