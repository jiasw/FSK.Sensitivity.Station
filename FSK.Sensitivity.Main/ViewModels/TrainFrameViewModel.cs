using FSK.Sensitivity.Core.Const;
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

        public TrainFrameViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
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
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_CheckHistory);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var s = regionManager.Regions[AppConst.TrainRegion].NavigationService.Journal;
            s.Clear();
            ShowLogButton = Visibility.Visible;
            // 通过Parameters属性访问
            if (navigationContext.Parameters.TryGetValue("type", out object? value))
            {
                string? data = value as string;
                if (data != null)
                {
                    if (data == "CSF")
                    {
                        regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_SensitivitySetting);
                    }
                    else if (data == "DEA")
                    {
                        regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_ContrastSetting);
                    }else if (data == "Setting")
                    {
                        ShowLogButton=Visibility.Collapsed;
                        regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_Setting);
                    }
                }

            }
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            // 通过Parameters属性访问
            if (navigationContext.Parameters.TryGetValue("type", out object? value))
            {
                string? data = value as string;
                //if (data != null)
                //{
                //    if (data == "CSF")
                //    {
                //        var s = regionManager.Regions[AppConst.TrainRegion].NavigationService;
                //        regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_SensitivitySetting);
                //    }
                //    else
                //    {
                //        regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_ContrastSetting);
                //    }
                //}

            }
        }
    }
}
