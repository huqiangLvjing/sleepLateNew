# 护眼小卫士 (SleepLate)

科学用眼，定时休息，保护视力健康。

## 功能特性

- 倒计时提醒（时:分:秒格式）
- 工作日/周末时间段设置
- 连续工作时长 + 每次休息时长设置
- 三种提醒模式：全屏锁定 / 弹窗提醒 / 静默提醒
- 管理员密码 + 超级密码（后门）双重保护
- 开机自动启动
- 统计今日使用时长和休息次数

## 技术栈

- C# / .NET 8
- WinForms
- 自包含单文件发布

## 项目结构

```
SleepLate/
├── App/                    # 应用入口
├── Data/                   # 数据层
│   ├── ConfigManager.cs   # 配置管理
│   └── Models/            # 数据模型
├── Services/              # 业务层
│   ├── TimerService.cs    # 定时器服务
│   ├── LockScreenService.cs  # 锁屏服务
│   ├── ReminderService.cs # 提醒服务
│   ├── PasswordService.cs # 密码服务
│   └── ...
├── UI/                    # UI层
│   ├── Forms/             # 窗体
│   └── Controls/         # 控件
└── Utils/                 # 工具层
    ├── Logger.cs          # 日志
    ├── Win32API.cs        # Win32 API
    └── EncryptUtil.cs     # 加密
```

## 构建

### 前置要求
- .NET 8 SDK

### 构建命令

```bash
cd SleepLate
dotnet build -c Release
```

### 发布单文件

```bash
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

## 设计文档

- [SDD.md](SDD.md) - 软件设计文档
- [TDD.md](TDD.md) - 测试设计文档

## 界面预览

### 主界面
- 渐变蓝紫导航栏
- 核心计时卡片（时:分:秒）
- 今日统计数据
- 时间段设置卡片 + 高级设置卡片
- 公告提示栏

### 退出验证
- 红色渐变头部
- 管理员密码 + 超级密码双输入
- 错误次数提示

## 更新日志

### v1.0.0
- 初始版本
- 完整的护眼提醒功能
- 密码保护机制