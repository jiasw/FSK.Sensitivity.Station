using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using FSK.Sensitivity.Main.Controls;
using FSK.Sensitivity.Main.Views.Dialogs;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class SystemSettingViewModel : BaseViewModel
    {
        private List<CheckInfo> _checkInfos;
        private readonly IRegionManager regionManager;
        private readonly IConfigurationService configurationService;

        public SystemSettingViewModel(IRegionManager regionManager,IConfigurationService configurationService)
        {
            this.regionManager = regionManager;
            this.configurationService = configurationService;
            
        }

        
        public DelegateCommand OpenFileSystemCommand => new DelegateCommand( () =>
        {
            Utils.StartProcess("explorer.exe");
        });

        public DelegateCommand RemoteSupportCommand => new DelegateCommand( () =>
        {
            string remoteSupportUrl = configurationService.LoadSetting().RemoteSupportPath;
            if (string.IsNullOrEmpty(remoteSupportUrl))
            {
                MessageBoxService.Instance.ShowInfo("未设置远程支持地址,请联系管理员!", "提示");
                return;
            }
            if (!File.Exists(remoteSupportUrl))
            {
                MessageBoxService.Instance.ShowInfo("未找到远程服务,请联系管理员!", "提示");
                return;
            }
            Utils.StartProcess(remoteSupportUrl);
        });


        public DelegateCommand SerialPortSettingCommand => new DelegateCommand( () =>
        {
            PasswordWindow passwordWindow = new PasswordWindow(configurationService);
            if (passwordWindow.ShowDialog() == true)
            {
                regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_SerialPortConfig);
            }

            
        });

        public DelegateCommand StoreSettingCommand => new DelegateCommand( () =>
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_Setting_Store);
        });
        public DelegateCommand UserInfoCommand => new DelegateCommand( () =>
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_Setting_UserInfo);
        });
        public DelegateCommand PatientInfoCommand => new DelegateCommand(() =>
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_Setting_Patient);
        });

        public DelegateCommand NetWorkCommand => new DelegateCommand(() =>
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_Setting_Netork);
        });

    }
}
