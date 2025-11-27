using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FSK.Sensitivity.Main.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ContrastConfigViewModel : BaseViewModel, INavigationAware
    {
        private readonly IRegionManager regionManager;
        
        public ContrastConfigViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            
        }

        private int _selectedTime = 5;
        public int SelectedTime
        {
            get => _selectedTime;
            set
            {
                SetProperty(ref _selectedTime, value);
            }
        }

        public DelegateCommand SaveCommand=> new DelegateCommand(Save);

        private void Save()
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_ContrastTraining,new NavigationParameters() { { "time", SelectedTime } });
        }

        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            regionManager.Regions[AppConst.MainRegion].NavigationService.Journal.GoBack();
        }

        public DelegateCommand ListResultCommand => new DelegateCommand(ListResult);

        private void ListResult()
        {
            // TODO: List the result of the test
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }
    }
}
