using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.EventBus;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace FSK.Sensitivity.Station
{
    public class SecondaryWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly SecondarySelectedEvent secondarySelectedEvent;

        public SecondaryWindowViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            secondarySelectedEvent = this.eventAggregator.GetEvent<SecondarySelectedEvent>();
            eventAggregator.GetEvent<SecondaryChangeEvent>().Subscribe(OnScreenChange);
        }
        
        public DelegateCommand<int> SelectedCommand => new DelegateCommand<int>(Selected);

        private void Selected(int obj)
        {
            secondarySelectedEvent.Publish(obj);
        }

        private void OnScreenChange(SecondaryChangeOptions options)
        {
            if (options.Action == ChangeAction.Idle)
            {
                regionManager.RequestNavigate(AppConst.SignRegion, AppConst.Sign_Page_Init);
            }
            else
            {
                regionManager.RequestNavigate(AppConst.SignRegion, AppConst.Sign_Page_Sensitivity);
            }

            
        }
    }
}
