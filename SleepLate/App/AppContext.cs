namespace SleepLate.App;

class AppContext
{
    private static AppContext? _instance;
    private static readonly object _lock = new();

    public static AppContext Instance => _instance ??= new AppContext();

    private AppContext() { }

    // Services
    public Data.ConfigManager ConfigManager { get; } = new();
    public Services.ConfigService ConfigService { get; private set; } = null!;
    public Services.TimerService TimerService { get; private set; } = null!;
    public Services.LockScreenService LockScreenService { get; private set; } = null!;
    public Services.ReminderService ReminderService { get; private set; } = null!;
    public Services.PasswordService PasswordService { get; private set; } = null!;
    public Services.StatisticsService StatisticsService { get; private set; } = null!;
    public Services.AutoStartService AutoStartService { get; private set; } = null!;

    public void Initialize()
    {
        ConfigService = new Services.ConfigService(ConfigManager);
        TimerService = new Services.TimerService(ConfigManager);
        LockScreenService = new Services.LockScreenService();
        ReminderService = new Services.ReminderService(LockScreenService);
        PasswordService = new Services.PasswordService();
        StatisticsService = new Services.StatisticsService();
        AutoStartService = new Services.AutoStartService(ConfigService.AutoStartEnabled);

        // Load passwords
        PasswordService.LoadPasswords(
            ConfigService.EncryptedAdminPassword,
            ConfigService.EncryptedSuperPassword,
            ConfigService.PasswordProtectionEnabled);

        // Set reminder mode
        ReminderService.CurrentMode = ConfigService.ReminderMode;
        ReminderService.SoundEnabled = ConfigService.SoundEnabled;
    }
}