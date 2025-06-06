using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.Model;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace FSK.Sensitivity.Main.ViewModels
{


    public class ArrowButtonStauts : BaseViewModel
    {
        private bool _lefButtonStatus = false;
        public bool LeftButtonStatus
        {
            get { return _lefButtonStatus; }
            set { SetProperty(ref _lefButtonStatus, value); }
        }

        private bool _rightButtonStatus = false;
        public bool RightButtonStatus
        {
            get { return _rightButtonStatus; }
            set { SetProperty(ref _rightButtonStatus, value); }
        }

        private bool _upButtonStatus = false;
        public bool UpButtonStatus
        {
            get { return _upButtonStatus; }
            set { SetProperty(ref _upButtonStatus, value); }
        }

        private bool _downButtonStatus = false;
        public bool DownButtonStatus
        {
            get { return _downButtonStatus; }
            set { SetProperty(ref _downButtonStatus, value); }
        }
        /// <summary>
        /// 顶部的确认按钮
        /// </summary>
        private bool _saveButtonStatus=false; 
        public bool SaveButtonStatus
        {
            get { return _saveButtonStatus; }
            set { SetProperty(ref _saveButtonStatus, value); }
        }
        /// <summary>
        /// 扳机按钮
        /// </summary>
        private bool _triggerButtonStatus=false;
        public bool TriggerButtonStatus
        {
            get { return _triggerButtonStatus; }
            set { SetProperty(ref _triggerButtonStatus, value); }
        }
        
    }

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


        private ArrowButtonStauts _arrowButtonStauts = new ArrowButtonStauts();
        public ArrowButtonStauts ArrowButtonStauts
        {
            get { return _arrowButtonStauts; }
            set { SetProperty(ref _arrowButtonStauts, value); }
        }

        private CheckUserModel _checkUserModel=new CheckUserModel();
        public CheckUserModel CheckUserModel
        {
            get { return _checkUserModel; }
            set { SetProperty(ref _checkUserModel, value); }
        }


        private int _leftCount=30;
        /// <summary>
        /// 左边倒计时
        /// </summary>
        public int LeftCount
        {
            get { return _leftCount; }
            set { SetProperty(ref _leftCount, value); }
        }

        private int _rightCount = 30;
        /// <summary>
        /// 右边倒计时
        /// </summary>
        public int RightCount
        {
            get { return _rightCount; }
            set { SetProperty(ref _rightCount, value); }
        }

        private bool _isTrainLeft;
        /// <summary>
        /// 训练左眼
        /// </summary>
        public bool IsTrainLeft
        {
            get { return _isTrainLeft; }
            set { SetProperty(ref _isTrainLeft, value); }
        }

        private bool _isTrainRight;
        /// <summary>
        /// 训练右眼
        /// </summary>
        public bool IsTrainRight
        {
            get { return _isTrainRight; }
            set { SetProperty(ref _isTrainRight, value); }
        }


        


        public DelegateCommand ShockCommand => new DelegateCommand(Shock);

        private void Shock()
        {
            Task.Run(async () =>
            {
                
                ArrowButtonStauts.DownButtonStatus = true;
                await Task.Delay(50);
                ArrowButtonStauts.DownButtonStatus = false;
            });
            
        }


        private ArrowButtonModel SaveButton { get; set; } = new ArrowButtonModel() { IsButtonActive = false, Name = "保存" };


        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            journal.GoBack();
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CheckUserModel = new CheckUserModel()
            {
                Id = AppData.Instance.Patient.Id,
                Name = AppData.Instance.Patient.PatientName,
                Gender = AppData.Instance.Patient.Gender,
                Age = AppData.Instance.Patient.Age.ToString(),
            };
            SensitivityConfigParam? sensitivityConfigParam = navigationContext.Parameters["sensitivityConfigParam"] as SensitivityConfigParam;
           if (sensitivityConfigParam != null)
            {
                CheckUserModel.PD = sensitivityConfigParam.PD.ToString();
                CheckUserModel.Light= sensitivityConfigParam.IsLightOnDisplay? "开" : "关";
            }
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
