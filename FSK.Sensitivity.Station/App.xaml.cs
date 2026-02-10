using FSK.Sensitivity.Main;
using FSK.Sensitivity.Main.Views;
using HandyControl.Tools;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

namespace FSK.Sensitivity.Station
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {
        private static Mutex _mutex;
        private const string MutexName = "FSK.Sensitivity.Station_Mutex_20260203";
        protected override void OnStartup(StartupEventArgs e)
        {
            // 判断程序是否已经启动
            bool isNew;
            _mutex = new Mutex(true, MutexName, out isNew);
            if (!isNew)
            {
                MessageBox.Show("应用程序已经运行！", "提示", MessageBoxButton.OK, MessageBoxImage.Warning);
                this.Shutdown();
                return;
            }
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    path: "Logs/app-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            // 捕获 UI 线程异常
            this.DispatcherUnhandledException += App_DispatcherUnhandledException;

            // 捕获非 UI 线程异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // 捕获异步异常
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            base.OnStartup(e);
            Log.Information("Application started");
        }
        protected override Window CreateShell()
        {
            

            return Container.Resolve<MainWindow>();
        }
       
        protected override void OnExit(ExitEventArgs e)
        {
            if (_mutex != null)
            {
                _mutex.ReleaseMutex();
                _mutex.Dispose();
            }
            base.OnExit(e);
        }
        protected override void OnInitialized()
        {
            base.OnInitialized();
            //设置handycontrol语言包，防止日期之类的控件显示英文
            ConfigHelper.Instance.SetLang("zh-CN");
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            Log.Debug("添加模块");
            //添加模块
            moduleCatalog.AddModule<SensitivityMainModule>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // 创建 LoggerFactory
            var loggerFactory = new LoggerFactory()
                .AddSerilog(Log.Logger);
            // 注册 ILoggerFactory
            containerRegistry.RegisterInstance<ILoggerFactory>(loggerFactory);
            // 创建通用 Logger<T> 供依赖注入使用
            containerRegistry.RegisterSingleton(
                typeof(ILogger<>),
                typeof(Logger<>));
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            LogException("DispatcherUnhandled", e.Exception);
            e.Handled = true; // 防止程序闪退
        }
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogException("AppDomain", e.ExceptionObject as Exception);
        }
        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            LogException("TaskScheduler", e.Exception);
            e.SetObserved();
        }
        private void LogException(string source, Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception in {Source}: {ExceptionType} - {Message}", source, ex.GetType().Name, ex.Message);
        }
    }

}
