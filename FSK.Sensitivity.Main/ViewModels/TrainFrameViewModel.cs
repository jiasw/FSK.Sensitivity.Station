using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.Utility;
using FSK.Sensitivity.Main.Views;
using Prism.Common;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class TrainFrameViewModel : BaseViewModel, INavigationAware
    {
        private readonly IRegionManager regionManager;
        Dictionary<MenuType, string> dictMenuType = new Dictionary<MenuType, string>();
        public TrainFrameViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            dictMenuType.Add(MenuType.CSF, AppConst.Main_Page_SensitivitySetting);
            dictMenuType.Add(MenuType.DEA, AppConst.Main_Page_ContrastSetting);
            dictMenuType.Add(MenuType.Setting, AppConst.Main_Page_Setting);
            dictMenuType.Add(MenuType.CSF_Train, AppConst.Main_Page_SensitivityTraining);
            dictMenuType.Add(MenuType.DEA_Train, AppConst.Main_Page_ContrastTraining);
            
            eventAggregator.GetEvent<CheckResultPageLoadRvent>().Subscribe(OnCheckResultPageLoad);

        }

        private void OnCheckResultPageLoad()
        {
            ShowLogButton = Visibility.Collapsed;
        }

        private Visibility showLogButton = Visibility.Visible;
        public Visibility ShowLogButton
        {
            get { return showLogButton; }
            set { SetProperty(ref showLogButton, value); }
        }


        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            string name = NavigationHelper.GetCurrentViewName(regionManager, AppConst.TrainRegion);
            if (name == AppConst.ResultWindowsName)
            {
                regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_Menu);
                return;
            }

            if (regionManager.Regions[AppConst.TrainRegion].NavigationService.Journal.CanGoBack)
            {
                regionManager.Regions[AppConst.TrainRegion].NavigationService.Journal.GoBack();
            }
            else
            {
                regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_Menu);
            }


        }

        public DelegateCommand ListResultCommand => new DelegateCommand(ListResult);

        private void ListResult()
        {
            
            RegionManagerExtensions.SafeRequestNavigate(regionManager, AppConst.TrainRegion, AppConst.Main_Page_CheckHistory);
        }



        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var navigationParameters = navigationContext.Parameters;
            var s = regionManager.Regions[AppConst.TrainRegion].NavigationService.Journal;
            s.Clear();
            // 通过Parameters属性访问
            if (!AppData.Instance.IsLogin)
            {
                ShowLogButton = Visibility.Collapsed;
            }

            if (navigationContext.Parameters.TryGetValue(nameof(MenuType), out object? value))
            {
                MenuType menu = (MenuType)value;
                regionManager.RequestNavigate(AppConst.TrainRegion, dictMenuType[menu], navigationParameters);
            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ShowLogButton = Visibility.Visible;
        }
    }
}
