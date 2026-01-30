@echo off
SETLOCAL

:: 1. 定义发布路径（当前目录下的 PublishOutput 文件夹）
SET OUTPUT_DIR=%~dp0PublishOutput

echo 开始发布 WPF 应用到: %OUTPUT_DIR%
echo 模式: 独立发布 (Self-contained), 不打包单文件

:: 2. 执行发布命令
:: -r 指定运行时，--self-contained 指定包含运行时
:: -p:PublishSingleFile=false 显式禁用单文件打包
dotnet publish -c Release -r win-x64 --self-contained true -o "%OUTPUT_DIR%" -p:PublishSingleFile=false -p:PublishReadyToRun=true

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ==========================================
    echo 发布成功！请查看目录: %OUTPUT_DIR%
    echo ==========================================
) else (
    echo.
    echo [错误] 发布失败，请检查错误日志。
)

pause