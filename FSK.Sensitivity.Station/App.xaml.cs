using FSK.Sensitivity.Main;
using FSK.Sensitivity.Main.Views;
using HandyControl.Tools;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Modularity;
using Serilog;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;

namespace FSK.Sensitivity.Station
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(
                    path: "Logs/app-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();
            base.OnStartup(e);
            Log.Information("Application started");
        }
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
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
    }

}
