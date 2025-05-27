using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.Model;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class SensitivityTrainingViewModel:BaseViewModel,INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IRegionNavigationJournal journal;
        private readonly SecondaryChangeEvent secondaryChangeEvent;

        public SensitivityTrainingViewModel(IRegionManager regionManager,IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.journal = regionManager.Regions[AppConst.MainRegion].NavigationService.Journal;
            secondaryChangeEvent = eventAggregator.GetEvent<SecondaryChangeEvent>();
        }
        private bool _isButtonActive;
        public bool IsButtonActive
        {
            get { return _isButtonActive; }
            set { SetProperty(ref _isButtonActive, value); }
        }


        private ArrowButtonModel SaveButton { get; set; } = new ArrowButtonModel() { IsButtonActive = false, Name = "保存" };


        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            journal.GoBack();
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Sensitivity, PicUrl = "" });
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Idle, PicUrl = "" });
        }

        public DelegateCommand ListResultCommand => new DelegateCommand(ListResult);

        private void ListResult()
        {
            // TODO: List the result of the test
        }
    }
}
