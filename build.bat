@echo off
echo ====================================
echo  护眼小卫士 - 构建脚本
echo ====================================

echo.
echo 检查 .NET SDK...
dotnet --version >nul 2>&1
if errorlevel 1 (
    echo [错误] 未找到 .NET SDK
    echo 请安装 .NET 8 SDK: https://dotnet.microsoft.com/download
    echo.
    echo 安装完成后，运行以下命令：
    echo   dotnet build -c Release
    pause
    exit /b 1
)

echo.
echo 开始构建...
cd /d "%~dp0SleepLate"
dotnet build -c Release

if errorlevel 1 (
    echo.
    echo [错误] 构建失败
    pause
    exit /b 1
)

echo.
echo ====================================
echo  构建成功！
echo ====================================
echo.
echo 可执行文件位于: SleepLate\bin\Release\net8.0-windows\SleepLate.exe
echo.
echo 发布单文件命令:
echo   dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
echo.
pause