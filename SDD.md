# SleepLate 软件设计文档 (SDD)

## 1. 项目概述

### 1.1 项目名称
**SleepLate** - 简洁锁屏工具

### 1.2 项目目标
一款极简主义的Windows锁屏工具，帮助用户管理屏幕锁定时间，避免过度使用电脑导致迟到。

### 1.3 核心功能
- 系统托盘常驻运行
- 双击托盘图标打开设置界面
- 可配置锁屏时间（分钟）
- 退出需输入密码验证
- 蓝白色调简洁UI

## 2. 技术架构

### 2.1 技术选型
- **语言**: C# (.NET Framework 4.8)
- **UI框架**: WinForms（轻量、无额外依赖）
- **构建工具**: MSBuild / dotnet CLI
- **打包**: 自包含单文件发布

### 2.2 设计原则
1. **零依赖**: 不依赖第三方库，点开即用
2. **最小体积**: Release构建单文件 < 10MB
3. **分层清晰**: UI层 / 业务层 / 数据层分离
4. **模块化**: 高内聚低耦合，便于维护

## 3. 系统架构

```
SleepLate/
├── App/                          # 入口模块
│   ├── Program.cs                 # 应用入口
│   └── AppContext.cs              # 应用上下文
├── UI/                           # UI层
│   ├── Forms/
│   │   ├── MainForm.cs           # 主窗体（设置界面）
│   │   └── PasswordForm.cs       # 密码验证窗体
│   └── Controls/
│       └── TimePickerControl.cs  # 时间选择控件
├── Services/                      # 业务层
│   ├── LockScreenService.cs      # 锁屏服务
│   ├── TimerService.cs           # 定时器服务
│   ├── PasswordService.cs        # 密码服务
│   └── ConfigService.cs          # 配置服务
├── Data/                         # 数据层
│   ├── ConfigManager.cs          # 配置管理
│   └── Models/
│       └── AppConfig.cs          # 配置模型
├── Utils/                        # 工具层
│   ├── Logger.cs                 # 日志工具
│   ├── Win32API.cs               # Win32 API封装
│   └── EncryptUtil.cs            # 加密工具
└── Resources/                     # 资源
    └── Icon.ico                  # 应用图标
```

## 4. 模块设计

### 4.1 UI层 (UI/)

#### 4.1.1 MainForm (主设置窗体)
- **职责**: 显示设置界面，响应用户操作
- **功能**:
  - 显示当前锁屏时间设置
  - 时间调整滑块/输入框
  - 开始/取消定时按钮
  - 退出按钮（触发密码验证）
- **日志**: 窗体打开/关闭、设置变更、定时开始/取消

#### 4.1.2 PasswordForm (密码验证窗体)
- **职责**: 验证用户密码
- **功能**:
  - 密码输入框
  - 确认/取消按钮
  - 错误提示
- **日志**: 验证成功/失败、尝试次数

### 4.2 业务层 (Services/)

#### 4.2.1 LockScreenService
- **职责**: 执行系统锁屏
- **方法**:
  - `Lock()`: 调用Windows锁屏API
- **日志**: 锁屏操作触发、执行结果

#### 4.2.2 TimerService
- **职责**: 管理倒计时定时器
- **方法**:
  - `Start(int minutes)`: 启动定时
  - `Stop()`: 停止定时
  - `OnTick`: 计时更新事件
  - `OnFinished`: 倒计时结束事件
- **日志**: 定时启动/停止、剩余时间变化

#### 4.2.3 PasswordService
- **职责**: 密码验证
- **方法**:
  - `Verify(string password)`: 验证密码
  - `SetPassword(string password)`: 设置密码
  - `IsPasswordSet()`: 检查是否已设置密码
- **日志**: 验证成功/失败

#### 4.2.4 ConfigService
- **职责**: 配置管理
- **方法**:
  - `Load()`: 加载配置
  - `Save()`: 保存配置
  - `GetDefaultLockMinutes()`: 获取默认锁屏分钟数
  - `SetDefaultLockMinutes(int minutes)`: 设置默认分钟数
- **日志**: 配置加载/保存

### 4.3 数据层 (Data/)

#### 4.3.1 ConfigManager
- **职责**: 配置文件的读写
- **存储位置**: `%APPDATA%\SleepLate\config.json`
- **日志**: 配置文件读写结果

#### 4.3.2 AppConfig
- **数据结构**:
```csharp
class AppConfig {
    int DefaultLockMinutes { get; set; }  // 默认锁屏分钟数
    string EncryptedPassword { get; set; } // 加密后的密码
    bool IsFirstRun { get; set; }          // 是否首次运行
}
```

### 4.4 工具层 (Utils/)

#### 4.4.1 Logger
- **职责**: 统一日志记录
- **日志级别**: DEBUG, INFO, WARN, ERROR
- **日志格式**: `[时间戳] [级别] [模块] 消息`
- **日志输出**: 文件 + 控制台
- **日志位置**: `%APPDATA%\SleepLate\logs\`

#### 4.4.2 Win32API
- **职责**: Windows API封装
- **方法**:
  - `LockWorkStation()`: 锁屏
  - `SetForegroundWindow()`: 激活窗口

#### 4.4.3 EncryptUtil
- **职责**: 密码加密
- **算法**: AES对称加密
- **日志**: 加密/解密操作

## 5. 界面设计

### 5.1 主界面 (MainForm)
```
+----------------------------------+
|  [蓝底白字] SleepLate 设置       |
+----------------------------------+
|                                  |
|   当前设置：将在 [15] 分钟后锁屏   |
|                                  |
|   [-  ]==========[+] 滑块选择     |
|                                  |
|   或直接输入: [____] 分钟         |
|                                  |
|   [开始定时]     [取消定时]        |
|                                  |
+----------------------------------+
|   剩余: 14:32                    |
|   [退出程序]                      |
+----------------------------------+
```

### 5.2 密码验证界面 (PasswordForm)
```
+----------------------------------+
|        [蓝底白字] 验证密码         |
+----------------------------------+
|                                  |
|   请输入密码:                     |
|   [********]                     |
|                                  |
|   [确定]      [取消]              |
|                                  |
|   (错误提示: 密码错误，剩余3次)     |
|                                  |
+----------------------------------+
```

## 6. 系统托盘

### 6.1 托盘图标
- **图标**: 蓝色时钟图标
- ** tooltip**: "SleepLate - 点击设置"

### 6.2 托盘菜单
- 右键菜单:
  - "打开设置" - 显示主界面
  - "立即锁屏" - 立即执行锁屏
  - "退出" - 退出程序（需密码）

## 7. 数据流

```
用户操作 → UI层 → 业务层 → 数据层
     ↓         ↓        ↓
  界面响应   业务逻辑   配置存储
```

### 7.1 定时锁屏流程
1. 用户设置锁屏分钟数
2. 点击"开始定时"
3. TimerService.Start() 启动倒计时
4. 倒计时结束触发 LockScreenService.Lock()
5. 系统锁屏

### 7.2 退出流程
1. 用户点击"退出程序"
2. 显示PasswordForm
3. 验证密码
4. 验证成功则退出程序
5. 验证失败显示错误，可重试

## 8. 异常处理

| 异常场景 | 处理方式 | 日志级别 |
|---------|---------|---------|
| 配置文件读取失败 | 使用默认值 | WARN |
| 配置文件写入失败 | 重试3次后提示 | ERROR |
| 锁屏API调用失败 | 记录错误，继续运行 | ERROR |
| 密码验证异常 | 返回false | DEBUG |

## 9. 配置项

| 配置项 | 类型 | 默认值 | 说明 |
|-------|------|-------|------|
| DefaultLockMinutes | int | 30 | 默认锁屏分钟数 |
| EncryptedPassword | string | "" | 加密密码（空=未设置） |
| IsFirstRun | bool | true | 是否首次运行 |

## 10. 日志规范

### 10.1 日志文件
- 位置: `%APPDATA%\SleepLate\logs\sleeplate_{date}.log`
- 滚动: 每天一个新文件，保留7天

### 10.2 日志格式
```
[2026-05-14 10:30:15] [INFO ] [TimerService] 定时器已启动，时长: 30分钟
[2026-05-14 10:30:16] [DEBUG] [LockScreenService] 用户取消锁屏
```

### 10.3 关键日志点
- 应用启动/退出
- 定时开始/取消/完成
- 锁屏执行
- 密码验证
- 配置加载/保存
- 异常错误