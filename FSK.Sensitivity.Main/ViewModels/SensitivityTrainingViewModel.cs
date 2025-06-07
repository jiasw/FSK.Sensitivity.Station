using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using Newtonsoft.Json.Linq;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

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
        private readonly SensitivitySignChangeEvent sensitivitySignChangeEvent;
        private readonly SecondarySelectedEvent sensitivitySignSelectedEvent;

        public SensitivityTrainingViewModel(IRegionManager regionManager,IEventAggregator eventAggregator)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.journal = regionManager.Regions[AppConst.MainRegion].NavigationService.Journal;
            secondaryChangeEvent = eventAggregator.GetEvent<SecondaryChangeEvent>();
            sensitivitySignChangeEvent= eventAggregator.GetEvent<SensitivitySignChangeEvent>();
            sensitivitySignSelectedEvent = eventAggregator.GetEvent<SecondarySelectedEvent>();
            sensitivitySignSelectedEvent.Subscribe(VaValueChanged);
        }

        

        private int imageIndex = 1;

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


        private BitmapImage _signImage;
        public BitmapImage SignImage
        {
            get { return _signImage; }
            set { SetProperty(ref _signImage, value); }
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
        /// <summary>
        /// 刷新视标
        /// </summary>
        public DelegateCommand RefreshCommand => new DelegateCommand(RefreshSignImage);

        private void RefreshSignImage()
        {
            string imageName = GetSignName();
            string imagePath = "pack://application:,,,/FSK.Sensitivity.Main;component/Resource/Images/Sign/" + imageName;
            BitmapImage bitmapImage = new BitmapImage(new Uri(imagePath));
            bitmapImage.Freeze();
            string signName = "t" + imageName.Substring(1);
            SignBackGround signBackGround = CheckUserModel.dayNight==Core.Enums.DayOrNight.Day? SignBackGround.White : SignBackGround.Black;
            sensitivitySignChangeEvent.Publish(new SensitivityChangeSignOptions() { PicturePath = signName,BackgroundBrush= signBackGround });
            SignImage = bitmapImage;
        }
        private string GetSignName()
        {
            
            string strdaycode = "d";
            if (CheckUserModel.dayNight == Core.Enums.DayOrNight.Night)
            {
                strdaycode = "k";
            }
            Dictionary<VAValue, string> dicVA = new Dictionary<VAValue, string>()
            {
                {VAValue.VA06, "06" },
                {VAValue.VA10, "1" },
                {VAValue.VA20, "2" },
                {VAValue.VA40, "4" },
                {VAValue.VA60, "6" },
                {VAValue.VA80, "8" },
            };
            int randomIndex = Utils.GenerateRandomNumber(1, 5, imageIndex);
            imageIndex = randomIndex;
            return $"d-{strdaycode}-{dicVA[_vaValue]}-{randomIndex}.jpg";
        }

        private VAValue _vaValue = VAValue.VA06;


        /// <summary>
        /// 用户选择视标事件
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void VaValueChanged(int obj)
        {
            Trace.WriteLine($"VA Value Changed:{obj}, vaValue:{_vaValue}");
            if(_vaValue == VAValue.VA80)
            {
                secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Idle });
            }
            NextVA();
        }

        private void NextVA()
        {
            if (_vaValue == VAValue.None || _vaValue == VAValue.VA80)
            {
                return;
            }
            Dictionary<VAValue, VAValue> dicVA = new Dictionary<VAValue, VAValue>()
            {
                {VAValue.VA06, VAValue.VA10 },
                {VAValue.VA10, VAValue.VA20 },
                {VAValue.VA20, VAValue.VA40 },
                {VAValue.VA40, VAValue.VA60 },
                {VAValue.VA60, VAValue.VA80 },
            };
            VAValue nextva = dicVA[_vaValue];

            _vaValue = nextva;
            RefreshSignImage();
        
        }



        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CheckUserModel model = new CheckUserModel()
            {
                Id = AppData.Instance.CurrentPatient.Id,
                Name = AppData.Instance.CurrentPatient.PatientName,
                Gender = AppData.Instance.CurrentPatient.Gender,
                Age = AppData.Instance.CurrentPatient.Age.ToString(),
            };
            SensitivityConfigParam? sensitivityConfigParam = navigationContext.Parameters["sensitivityConfigParam"] as SensitivityConfigParam;
           if (sensitivityConfigParam != null)
            {
                model.PD = sensitivityConfigParam.PD.ToString();
                model.Light= sensitivityConfigParam.IsLightOnDisplay? "开" : "关";
                model.dayNight = sensitivityConfigParam.DayNight;
            }
            CheckUserModel = model;
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Sensitivity, Brush= CheckUserModel.dayNight == Core.Enums.DayOrNight.Day ? SignBackGround.White : SignBackGround.Black });
            RefreshSignImage();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Idle });
        }

        public DelegateCommand ListResultCommand => new DelegateCommand(ListResult);

        private void ListResult()
        {
            // TODO: List the result of the test
        }
    }
}
