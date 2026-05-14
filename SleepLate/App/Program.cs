using SleepLate.App;
using SleepLate.Utils;

namespace SleepLate.App;

static class Program
{
    private static readonly Logger _logger = Logger.Instance;

    [STAThread]
    static void Main()
    {
        _logger.Info("App", "===========================================");
        _logger.Info("App", $"护眼小卫士 v1.0.0 启动");
        _logger.Info("App", "===========================================");

        try
        {
            AppContext.Instance.Initialize();
            _logger.Info("App", "服务初始化完成");

            ApplicationConfiguration.Initialize();
            Application.Run(new UI.Forms.MainForm());
        }
        catch (Exception ex)
        {
            _logger.Error("App", $"应用程序异常: {ex.Message}");
            MessageBox.Show($"应用程序启动失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        _logger.Info("App", "应用程序退出");
    }
}