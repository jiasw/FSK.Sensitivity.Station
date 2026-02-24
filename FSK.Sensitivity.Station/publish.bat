@echo off
SETLOCAL ENABLEDELAYEDEXPANSION

:: 1. 定义路径变量
SET "OUTPUT_DIR=%~dp0PublishOutput"
SET "REMOTE_DIR=\\192.168.1.38\对比敏感度\PublishOutput"
SET "PROJECT_NAME=对比敏感度"

echo ==========================================
echo WPF 应用发布脚本
echo ==========================================
echo 发布目录: %OUTPUT_DIR%
echo 远程目录: %REMOTE_DIR%
echo 发布模式: 独立发布 (Self-contained)
echo 单文件打包: 已禁用
echo ==========================================

:: 2. 检查源目录是否存在
if not exist "%OUTPUT_DIR%" (
    echo 创建发布目录...
    mkdir "%OUTPUT_DIR%" 2>nul
    if errorlevel 1 (
        echo [错误] 无法创建发布目录: %OUTPUT_DIR%
        goto :error
    )
)

:: 3. 清理旧的发布文件
echo.
echo 清理旧的发布文件...
if exist "%OUTPUT_DIR%\*" (
    del /Q "%OUTPUT_DIR%\*" >nul 2>&1
    for /D %%i in ("%OUTPUT_DIR%\*") do rmdir /Q "%%i" >nul 2>&1
)

:: 4. 执行发布命令
echo.
echo 开始编译和发布...
echo.

dotnet publish -c Release -r win-x64 --self-contained true -o "%OUTPUT_DIR%" -p:PublishSingleFile=false -p:PublishReadyToRun=true

if %ERRORLEVEL% NEQ 0 (
    echo.
    echo [错误] 编译发布失败！
    goto :error
)

:: 5. 检查发布结果
echo.
echo 检查发布文件...
set "file_count=0"
if exist "%OUTPUT_DIR%" (
    for %%i in ("%OUTPUT_DIR%\*.*") do set /a file_count+=1
)

if %file_count% EQU 0 (
    echo [错误] 发布目录为空，发布可能失败！
    goto :error
)

echo 发布文件数量: %file_count% 个

:: 6. 复制到远程目录
REM echo.
REM echo 正在复制到远程目录...
REM echo 源目录: %OUTPUT_DIR%
REM echo 目标目录: %REMOTE_DIR%

REM :: 检查远程目录是否可访问
REM if not exist "%REMOTE_DIR%" (
    REM echo [警告] 远程目录不可访问，尝试重新连接...
    REM net use "%REMOTE_DIR%" >nul 2>&1
    REM if not exist "%REMOTE_DIR%" (
        REM echo [错误] 无法访问远程目录: %REMOTE_DIR%
        REM echo 请检查网络连接和共享权限
        REM goto :warning
    REM )
REM )

REM :: 使用 robocopy 进行文件复制（更可靠）
REM robocopy "%OUTPUT_DIR%" "%REMOTE_DIR%" /E /IS /R:3 /W:5 /XF FSK.db /XD Resource


REM :: robocopy 的返回码含义：0-3 表示成功，大于3表示有错误
REM if %ERRORLEVEL% GTR 3 (
    REM echo.
    REM echo [警告] 文件复制过程中出现一些问题 (错误码: %ERRORLEVEL%)
    REM echo 部分文件可能未成功复制
    REM goto :warning
REM ) else (
    REM echo.
    REM echo 文件复制成功！
REM )

:: 7. 成功完成
goto :success

:: ================================
:: 结果处理
:: ================================
:success
echo.
echo ==========================================
echo 发布成功完成！
echo ==========================================
echo 本地目录: %OUTPUT_DIR%
echo 远程目录: %REMOTE_DIR%
echo 文件数量: %file_count% 个
echo 发布时间: %date% %time%
echo ==========================================
goto :end

:warning
echo.
echo ==========================================
echo 发布完成，但有警告信息
echo ==========================================
echo 请检查上述警告内容
echo ==========================================
goto :end

:error
echo.
echo ==========================================
echo 发布失败！
echo ==========================================
echo 请检查错误信息并重新尝试
echo ==========================================
exit /b 1

:end
echo.
echo 脚本执行完毕
pause
