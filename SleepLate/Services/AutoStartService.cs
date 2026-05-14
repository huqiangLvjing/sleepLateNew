using Microsoft.Win32;
using SleepLate.Utils;

namespace SleepLate.Services;

class AutoStartService
{
    private static readonly Logger _logger = Logger.Instance;
    private const string AppName = "SleepLate";
    private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    public bool IsEnabled { get; private set; } = false;

    public AutoStartService(bool enabled)
    {
        IsEnabled = enabled;
    }

    public void Enable()
    {
        try
        {
            var exePath = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath)) return;

            using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
            key?.SetValue(AppName, $"\"{exePath}\"");
            IsEnabled = true;

            _logger.Info("AutoStartService", "开机自动启动已启用");
        }
        catch (Exception ex)
        {
            _logger.Error("AutoStartService", $"启用开机启动失败: {ex.Message}");
        }
    }

    public void Disable()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
            key?.DeleteValue(AppName, false);
            IsEnabled = false;

            _logger.Info("AutoStartService", "开机自动启动已禁用");
        }
        catch (Exception ex)
        {
            _logger.Error("AutoStartService", $"禁用开机启动失败: {ex.Message}");
        }
    }

    public void SetEnabled(bool enabled)
    {
        if (enabled)
            Enable();
        else
            Disable();
    }
}