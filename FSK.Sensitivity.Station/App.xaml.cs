using FSK.Sensitivity.Main.Views;
using System.Configuration;
using System.Data;
using System.Windows;
using Prism.Ioc;
using Prism.Modularity;
using HandyControl.Tools;
using FSK.Sensitivity.Main;
using System.Linq;
using System.Collections.Generic;

namespace FSK.Sensitivity.Station
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App 
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
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
            
        }
    }

}
