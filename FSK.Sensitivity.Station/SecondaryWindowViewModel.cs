using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.EventBus;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Prism.Ioc;

namespace FSK.Sensitivity.Station
{
    public class SecondaryWindowViewModel : BindableBase, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;

        public SecondaryWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            eventAggregator.GetEvent<SecondaryChangeEvent>().Subscribe(OnScreenChange);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return false;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        private void OnScreenChange(SecondaryChangeOptions options)
        {
            if (options.Action == ChangeAction.Sensitivity)
            {
                regionManager.RequestNavigate(AppConst.SignRegion, AppConst.Sign_Page_Sensitivity);
            }
            else if (options.Action == ChangeAction.Contrast)
            {
                regionManager.RequestNavigate(AppConst.SignRegion, AppConst.Sign_Page_Contrast);
            }
            else
            {
                regionManager.RequestNavigate(AppConst.SignRegion, AppConst.Sign_Page_Init);
            }

        }
    }
}
