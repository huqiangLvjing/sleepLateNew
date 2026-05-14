# SleepLate (护眼小卫士) 软件设计文档 (SDD)

## 1. 项目概述

### 1.1 项目名称
**护眼小卫士** (SleepLate)

### 1.2 项目目标
一款简约护眼提醒工具，帮助用户科学用眼、定时休息，通过锁屏等方式强制用户适时休息，保护视力健康。

### 1.3 核心功能
- 倒计时提醒（时:分:秒格式）
- 工作日/周末时间段设置
- 连续工作时长 + 每次休息时长设置
- 三种提醒模式：全屏锁定 / 弹窗提醒 / 静默提醒
- 管理员密码 + 超级密码（后门）双重保护
- 开机自动启动
- 统计今日使用时长和休息次数

## 2. 技术架构

### 2.1 技术选型
- **语言**: C# (.NET 6/8)
- **UI框架**: WinForms（轻量、无额外依赖）
- **构建工具**: dotnet CLI
- **打包**: 自包含单文件发布

### 2.2 设计原则
1. **零依赖**: 不依赖第三方库，点开即用
2. **最小体积**: Release构建单文件 < 15MB
3. **分层清晰**: UI层 / 业务层 / 数据层分离
4. **模块化**: 高内聚低耦合，便于维护

## 3. 系统架构

```
SleepLate/
├── App/                          # 入口模块
│   ├── Program.cs                # 应用入口
│   └── AppContext.cs             # 应用上下文
├── UI/                           # UI层
│   ├── Forms/
│   │   ├── MainForm.cs           # 主界面
│   │   ├── TimePeriodForm.cs     # 时间段设置页面
│   │   ├── AdvancedSettingsForm.cs # 高级设置页面
│   │   └── PasswordForm.cs       # 退出验证页面
│   └── Controls/
│       ├── TimerDisplayControl.cs     # 倒计时显示控件
│       ├── TimeInputControl.cs        # 时分秒输入控件
│       ├── SettingsCardControl.cs     # 设置卡片控件
│       └── NoticeBarControl.cs        # 公告提示栏控件
├── Services/                      # 业务层
│   ├── TimerService.cs           # 定时器服务（核心）
│   ├── LockScreenService.cs      # 锁屏服务
│   ├── ReminderService.cs        # 提醒服务
│   ├── PasswordService.cs       # 密码服务
│   ├── ConfigService.cs          # 配置服务
│   ├── StatisticsService.cs      # 统计服务
│   └── AutoStartService.cs       # 开机启动服务
├── Data/                         # 数据层
│   ├── ConfigManager.cs          # 配置管理（JSON文件）
│   └── Models/
│       ├── AppConfig.cs          # 应用配置模型
│       ├── TimePeriod.cs         # 时间段模型
│       └── DailyStatistics.cs    # 每日统计模型
├── Utils/                        # 工具层
│   ├── Logger.cs                 # 日志工具
│   ├── Win32API.cs               # Win32 API封装
│   ├── EncryptUtil.cs            # 加密工具（AES）
│   └── TimeValidator.cs          # 时间验证工具
└── Resources/                    # 资源
    └── Icon.ico                  # 应用图标
```

## 4. 全局设计规范

### 4.1 整体风格
- 现代圆角卡片式UI
- 明亮商务简约风格
- 层次分明，柔和轻量化阴影
- 视觉干净清爽

### 4.2 统一样式
| 属性 | 值 |
|------|-----|
| 通用圆角 | 12px ~ 16px |
| 卡片样式 | 纯白色背景 + 浅灰色柔和投影 |
| 全局背景色 | #f5f7fa |
| 字体 | 无衬线常规字体 |

### 4.3 标准色值
| 用途 | 色值 |
|------|-----|
| 顶部导航渐变主色 | #1677ff → #722ed1 |
| 成功状态绿色 | #52c41a |
| 警告提示黄色 | #faad14 |
| 危险操作红色 | #ff4d4f |
| 正文主要文字色 | #1f2329 |
| 辅助灰色文字色 | #86909c |
| 卡片纯白底色 | #ffffff |

## 5. 界面设计

### 5.1 主界面 (MainForm)

```
+--------------------------------------------------+
|  [渐变蓝紫导航栏]                                  |
|  🕐 护眼小卫士    科学用眼·定时休息    [运行中] ● |
+--------------------------------------------------+
|                                                  |
|  [核心计时卡片 - 居中浅蓝底]                       |
|                                                  |
|       距离下次休息还有                            |
|                                                  |
|      [时] : [分] : [秒]                          |
|       小时   分钟   秒                           |
|                                                  |
|  今日已使用电脑: 2h30m    今日已休息: 3次         |
|                                                  |
+--------------------------------------------------+
|  [时间段设置卡片]        |  [高级设置卡片]        |
|  📅 工作日: 09:00-18:00  |  ⚙️ 休息时长: 5分钟    |
|       周末: 10:00-22:00  |  工作时长: 45分钟      |
|                    →     |  退出保护: 开启 →       |
+--------------------------------------------------+
|  [黄色公告提示栏]                                 |
|  ⚠️ 本程序已启用防退出保护，退出程序需要输入       |
|     管理员密码。请牢记设置的密码，超级密码可紧急解锁|
+--------------------------------------------------+
|  SleepLate v1.0.0          [_最小化] [×退出程序]  |
+--------------------------------------------------+
```

### 5.2 时间段设置页面 (TimePeriodForm)

```
+--------------------------------------------------+
|  [← 返回]  工作时间设置                            |
|         设置提醒时间与生效时段                      |
+--------------------------------------------------+
|                                                  |
|  [温馨说明卡片]                                    |
|  💡 仅在设置时间段内休息提醒生效                   |
|                                                  |
|  [提醒间隔设置 - 左右双列]                         |
|  连续工作时长: [-] [45] [+] 分钟                   |
|  底部: 建议工作45分钟后休息一次                    |
|                                                  |
|  每次休息时长: [-] [5] [+] 分钟                    |
|  底部: 休息5分钟保护眼睛健康                       |
|                                                  |
|  [生效时间段管理]                                 |
|  工作日时段  [开启] 周一至周五  09:00-18:00  ✏️🗑️ |
|  周末时段    [开启] 周六周日  10:00-22:00   ✏️🗑️ |
|                               [+ 添加时段]        |
|                                                  |
|  [提醒方式选择]                                   |
|  ○ 全屏锁定提醒 - 强制锁屏保护眼睛                  |
|  ○ 弹窗提醒     - 提示后继续工作                   |
|  ○ 静默提醒     - 只记录不打扰                      |
|                                                  |
|  ☑ 提醒音效开启                                  |
|                                                  |
|      [取消]              [保存设置]               |
+--------------------------------------------------+
```

### 5.3 退出验证页面 (PasswordForm)

```
+--------------------------------------------------+
|  [红色渐变头部]                                    |
|                                                  |
|       🔒                                        |
|       身份验证                                   |
|   请输入管理员密码以退出程序                        |
|                                                  |
|  [黄色警告提示框]                                 |
|  ⚠️ 连续3次错误输入将锁定程序                      |
|     超级密码可用于紧急解锁                         |
|                                                  |
|  管理员密码                                       |
|  🔒 [****************] 👁                          |
|  错误: 密码错误，剩余3次                           |
|  辅助: 忘记密码？使用超级密码                       |
|                                                  |
|  超级密码（后门）                                 |
|  🔑 [________________]                            |
|  万能密码可无条件解锁程序                          |
|                                                  |
|      [取消]            [确认退出]                 |
+--------------------------------------------------+
```

### 5.4 高级设置页面 (AdvancedSettingsForm)

```
+--------------------------------------------------+
|  [渐变导航栏]                                     |
|  [← 返回]  高级设置                                |
|         密码与系统配置                             |
+--------------------------------------------------+
|                                                  |
|  [浅红警示提示栏]                                 |
|  ⚠️ 请妥善保管管理员密码与超级密码                 |
|                                                  |
|  [密码管理配置卡片]                               |
|  管理员密码           功能描述      [修改密码]    |
|  超级后门密码         功能描述      [修改密码]    |
|  退出密码保护         功能描述      [开关●]      |
|  开机自动启动         功能描述      [开关○]      |
|                                                  |
|  [扩展配置区域 - 预留]                            |
|  护眼规则 / 全局音效 / 界面样式                   |
|                                                  |
|  [关于信息卡片]                                   |
|  护眼小卫士 v1.0.0                                |
|  科学用眼，定时休息，保护视力健康                  |
|                                                  |
|      [取消]              [保存设置]               |
+--------------------------------------------------+
```

## 6. 模块详细设计

### 6.1 业务层 (Services/)

#### TimerService - 核心定时器服务
```csharp
class TimerService {
    // 状态
    TimeSpan RemainingTime { get; }      // 剩余时间
    TimerState State { get; }             // 运行中/已暂停/已停止

    // 核心方法
    void Start(int workMinutes);          // 开始计时（工作周期）
    void Pause();                         // 暂停
    void Resume();                        // 恢复
    void Stop();                          // 停止
    void Reset();                         // 重置

    // 事件
    event EventHandler<TimeSpan> OnTick;           // 每秒触发
    event EventHandler OnWorkFinished;             // 工作时间到，触发休息提醒
    event EventHandler OnRestFinished;              // 休息时间到
    event EventHandler OnDayReset;                  // 日期变更，重置统计
}
```

#### LockScreenService - 锁屏服务
```csharp
class LockScreenService {
    void Lock();                          // 执行系统锁屏
    void ShowFullScreenReminder();        // 全屏提醒模式
    void HideFullScreenReminder();        // 关闭全屏提醒
    bool IsFullScreenShowing { get; }
}
```

#### ReminderService - 提醒服务
```csharp
enum ReminderMode {
    FullScreenLock,   // 全屏锁定
    Popup,            // 弹窗提醒
    Silent            // 静默
}

class ReminderService {
    ReminderMode CurrentMode { get; set; }
    bool SoundEnabled { get; set; }

    void TriggerReminder();               // 触发提醒
    void PlaySound();                     // 播放提示音
}
```

#### PasswordService - 密码服务
```csharp
class PasswordService {
    bool VerifyAdminPassword(string pwd);     // 验证管理员密码
    bool VerifySuperPassword(string pwd);     // 验证超级密码
    void SetAdminPassword(string pwd);        // 设置管理员密码
    void SetSuperPassword(string pwd);         // 设置超级密码
    bool IsPasswordProtectionEnabled { get; set; }
    int FailedAttempts { get; }               // 失败次数
    bool IsLocked { get; }                    // 是否已锁定
}
```

#### ConfigService - 配置服务
```csharp
class ConfigService {
    AppConfig Load();                      // 加载配置
    void Save(AppConfig config);           // 保存配置

    // 配置项访问
    int WorkMinutes { get; set; }         // 连续工作时长
    int RestMinutes { get; set; }          // 每次休息时长
    List<TimePeriod> TimePeriods { get; }  // 生效时间段
    ReminderMode Mode { get; set; }        // 提醒模式
}
```

#### StatisticsService - 统计服务
```csharp
class StatisticsService {
    TimeSpan TodayUsageTime { get; }       // 今日使用时长
    int TodayRestCount { get; }            // 今日休息次数

    void RecordUsage(TimeSpan duration);   // 记录使用时长
    void RecordRest();                      // 记录一次休息
    void ResetDaily();                      // 重置每日统计
}
```

#### AutoStartService - 开机启动服务
```csharp
class AutoStartService {
    bool IsEnabled { get; set; }
    void Enable();                         // 启用开机启动
    void Disable();                        // 禁用开机启动
}
```

### 6.2 数据层 (Data/)

#### AppConfig - 应用配置
```csharp
class AppConfig {
    int WorkMinutes { get; set; }         // 默认: 45
    int RestMinutes { get; set; }          // 默认: 5
    string EncryptedAdminPassword { get; set; }
    string EncryptedSuperPassword { get; set; }
    bool PasswordProtectionEnabled { get; set; }
    bool AutoStartEnabled { get; set; }
    ReminderMode ReminderMode { get; set; }
    bool SoundEnabled { get; set; }
    List<TimePeriod> TimePeriods { get; set; }
    DateTime LastResetDate { get; set; }
}
```

#### TimePeriod - 时间段
```csharp
class TimePeriod {
    string Name { get; set; }              // 时段名称
    DayOfWeek[] Days { get; set; }        // 生效星期
    TimeSpan StartTime { get; set; }       // 开始时间
    TimeSpan EndTime { get; set; }         // 结束时间
    bool Enabled { get; set; }
}
```

#### DailyStatistics - 每日统计
```csharp
class DailyStatistics {
    DateTime Date { get; set; }
    TimeSpan TotalUsageTime { get; set; }
    int RestCount { get; set; }
}
```

### 6.3 工具层 (Utils/)

#### Logger - 日志工具
```
位置: %APPDATA%\SleepLate\logs\sleeplate_{date}.log
滚动: 每天新文件，保留7天
格式: [时间戳] [级别] [模块] 消息
```

#### Win32API - Win32封装
```csharp
class Win32API {
    [DllImport("user32.dll")]
    bool LockWorkStation();                // 锁屏

    [DllImport("user32.dll")]
    bool SetForegroundWindow(IntPtr hWnd); // 激活窗口
}
```

#### EncryptUtil - 加密工具
```csharp
class EncryptUtil {
    string Encrypt(string plainText);     // AES加密
    string Decrypt(string cipherText);     // AES解密
}
```

## 7. 数据流与交互

### 7.1 核心计时流程
```
应用启动
    ↓
ConfigService.Load() → 加载配置
    ↓
TimerService.Start(workMinutes) → 启动倒计时
    ↓
TimerService.OnTick(每秒) → 更新UI倒计时显示
    ↓
剩余时间归零
    ↓
根据ReminderMode执行:
  - FullScreenLock → LockScreenService.Lock()
  - Popup → 显示弹窗提醒
  - Silent → 仅记录统计
    ↓
休息倒计时开始 → RestMinutes
    ↓
休息结束 → 统计+1，TimerService.Start() 继续工作周期
```

### 7.2 退出验证流程
```
用户点击退出
    ↓
检查PasswordProtectionEnabled
    ↓
显示PasswordForm
    ↓
用户输入密码
    ↓
PasswordService.VerifyAdminPassword() 或 VerifySuperPassword()
    ↓
验证成功 → 退出程序
验证失败 → 显示错误提示，剩余次数-1
    ↓
3次失败 → 显示锁定提示
```

### 7.3 时间段检测流程
```
每分钟检测一次
    ↓
检查当前时间是否在任一时间段内
    ↓
时间段有效 → TimerService继续运行
时间段无效 → TimerService暂停，显示"非工作时间"
```

## 8. 配置文件

路径: `%APPDATA%\SleepLate\config.json`

```json
{
  "workMinutes": 45,
  "restMinutes": 5,
  "encryptedAdminPassword": "",
  "encryptedSuperPassword": "",
  "passwordProtectionEnabled": false,
  "autoStartEnabled": false,
  "reminderMode": "FullScreenLock",
  "soundEnabled": true,
  "timePeriods": [
    {
      "name": "工作日",
      "days": [1, 2, 3, 4, 5],
      "startTime": "09:00",
      "endTime": "18:00",
      "enabled": true
    },
    {
      "name": "周末",
      "days": [0, 6],
      "startTime": "10:00",
      "endTime": "22:00",
      "enabled": true
    }
  ],
  "lastResetDate": "2026-05-14"
}
```

## 9. 日志规范

### 9.1 日志级别
- **DEBUG**: 详细调试信息
- **INFO**: 一般信息（启动、停止、功能执行）
- **WARN**: 警告（配置缺失、验证失败）
- **ERROR**: 错误（异常、API调用失败）

### 9.2 关键日志点
| 操作 | 级别 | 内容 |
|------|------|------|
| 应用启动 | INFO | SleepLate started, version {version} |
| 应用退出 | INFO | SleepLate exiting normally |
| 定时开始 | INFO | Timer started, workMinutes={n} |
| 定时停止 | INFO | Timer stopped |
| 倒计时更新 | DEBUG | Timer tick, remaining={hh:mm:ss} |
| 工作时间到 | INFO | Work period finished, starting rest |
| 休息时间到 | INFO | Rest period finished |
| 锁屏执行 | INFO | LockScreen executed |
| 密码验证 | INFO | Password verification {success/failed} |
| 配置加载 | DEBUG | Config loaded from {path} |
| 配置保存 | DEBUG | Config saved to {path} |
| 错误异常 | ERROR | {exception message} |

## 10. 异常处理

| 场景 | 处理方式 | 日志级别 |
|------|---------|---------|
| 配置文件不存在 | 使用默认值创建 | WARN |
| 配置文件损坏 | 使用默认值，回退初始状态 | ERROR |
| 配置文件保存失败 | 重试3次后提示用户 | ERROR |
| 锁屏API失败 | 记录错误，程序继续运行 | ERROR |
| 密码验证异常 | 返回false | DEBUG |
| 时间段重叠 | 使用最新的设置 | WARN |

## 11. 安全设计

### 11.1 密码存储
- 密码使用AES加密存储
- 加密密钥硬编码在程序中
- 超级密码作为后门，可无条件解锁

### 11.2 退出保护
- 启用后退出需要验证密码
- 连续3次失败锁定（需重启或用超级密码）

### 11.3 防破解
- 程序不存储明文密码
- 加密算法不可逆