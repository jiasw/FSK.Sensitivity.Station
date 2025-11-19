
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Infrastructure
{
    public sealed class LogHelper 
    {
        private static readonly Lazy<LogHelper> _instance = new Lazy<LogHelper>(() => new LogHelper());
        private ILogger _logger;
        public static LogHelper Instance => _instance.Value;
        public ILogger Logger => _logger;
        private LogHelper()
        {
            InitializeLogger();
        }
        private void InitializeLogger()
        {
            // 获取应用程序根目录
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");

            // 确保日志目录存在
            if (!Directory.Exists(logPath))
            {
                Directory.CreateDirectory(logPath);
            }
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    path: Path.Combine(logPath, "app-.log"),
                    rollingInterval: RollingInterval.Day,
                    retainedFileCountLimit: 30,
                    fileSizeLimitBytes: 10_000_000,
                    rollOnFileSizeLimit: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
        }
        public void LogInformation(string message)
        {
            _logger.Information(message);
        }
        public void LogWarning(string message)
        {
            _logger.Warning(message);
        }
        public void LogError(string message, Exception? exception = null)
        {
            if (exception != null)
                _logger.Error(exception, message);
            else
                _logger.Error(message);
        }
        public void LogDebug(string message)
        {
            _logger.Debug(message);
        }
        public void LogVerbose(string message)
        {
            _logger.Verbose(message);
        }
        public void LogFatal(string message, Exception? exception = null)
        {
            if (exception != null)
                _logger.Fatal(exception, message);
            else
                _logger.Fatal(message);
        }
        // 应用程序关闭时调用，确保日志被正确写入
        public void Dispose()
        {
            (_logger as Logger)?.Dispose();
        }
    }
}
