@echo off
echo ======================================================
echo   WPF Project Publishing: .NET 9.0 Self-Contained
echo   Target: win-x64 (Folder Mode)
echo ======================================================

:: 1. 设置变量
set CONFIG=Release
set RUNTIME=win-x64
set OUTPUT_DIR=.\publish_output

:: 2. 清理旧的发布文件夹
if exist %OUTPUT_DIR% (
    echo Cleaning old files...
    rd /s /q %OUTPUT_DIR%
)

:: 3. 执行发布命令
:: -c: 配置模式
:: -r: 目标平台
:: --self-contained: 独立部署
:: -p:PublishSingleFile=false: 不打包成单文件
:: -o: 指定输出目录
echo Publishing... Please wait...
dotnet publish -c %CONFIG% -r %RUNTIME% --self-contained true -p:PublishSingleFile=false -o %OUTPUT_DIR%

:: 4. 检查是否成功
if %ERRORLEVEL% EQU 0 (
    echo.
    echo ======================================================
    echo   PUBLISH SUCCESSFUL!
    echo   Output: %cd%\%OUTPUT_DIR%
    echo ======================================================
    start %OUTPUT_DIR%
) else (
    echo.
    echo ********** ERROR OCCURRED DURING PUBLISH **********
    pause
)

exit