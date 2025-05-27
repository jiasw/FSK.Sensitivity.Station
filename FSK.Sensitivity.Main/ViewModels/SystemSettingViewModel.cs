using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
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
        private readonly AppSetting appSetting;

        public List<CheckInfo> CheckInfos
        {
            get { return _checkInfos; }
            set { SetProperty(ref _checkInfos, value); }
        }
        public SystemSettingViewModel(IRegionManager regionManager, AppSetting appSetting)
        {
            List < CheckInfo > lists = new List<CheckInfo>();
            lists.Add(new CheckInfo() { Item="Item1",Result="DDDD",Status="OK" });
            lists.Add(new CheckInfo() { Item="Item2",Result="EEEEE",Status="NG" });
            lists.Add(new CheckInfo() { Item="Item3",Result="FFFFFF",Status="OK" });
            lists.Add(new CheckInfo() { Item="Item4",Result="GGGGGG",Status="NG" });
            CheckInfos  = lists;
            this.regionManager = regionManager;
            this.appSetting = appSetting;
        }

        
        public DelegateCommand OpenFileSystemCommand => new DelegateCommand( () =>
        {
            Utils.StartProcess("explorer.exe");
        });

        public DelegateCommand RemoteSupportCommand => new DelegateCommand( () =>
        {
            string remoteSupportUrl = appSetting.RemoteSupportPath;
            if (string.IsNullOrEmpty(remoteSupportUrl))
            {
                return;
            }
            if (!File.Exists(remoteSupportUrl))
            {
                return;
            }
            Utils.StartProcess(remoteSupportUrl);
        });


        public DelegateCommand SerialPortSettingCommand => new DelegateCommand( () =>
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_SerialPortConfig);
        });

        public DelegateCommand StoreSettingCommand => new DelegateCommand( () =>
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_Setting_Store);
        });
        public DelegateCommand UserInfoCommand => new DelegateCommand( () =>
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_Setting_UserInfo);
        });
    }
}
