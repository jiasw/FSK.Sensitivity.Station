using Dm;
using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using FSK.Sensitivity.Main.Controls;
using NetTaste;
using Prism.Events;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FSK.Sensitivity.Main.ViewModels
{

  

    /*
     暗环境适应逻辑：
     1. 白屏等待30秒
     2. 等待时长结束，开始检查时长

     
     */
    
    public class ContrastTrainingViewModel : BaseViewModel, INavigationAware
    {
        private readonly SecondaryChangeEvent secondaryChangeEvent;//副屏页面切换事件
        private readonly SensitivitySignChangeEvent contrastSignChangeEvent;//对比度标志位事件
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly ILight light;
        private readonly ISpeechService speechService;
        private readonly ITrainingAndCheckService trainingAndCheckService;
        private CheckUserModel _checkUserModel = new CheckUserModel();

        ContrastConfigParam contrastConfigParam;
        //选择视标的索引
        private int _signSelectIndex = 0;

        /// <summary>
        /// 训练模式
        /// </summary>
        private TrainEnterMode CurrentTrainEnterMode=TrainEnterMode.Normal;

        public int SignSelectIndex
        {
            get { return _signSelectIndex; }
            set { SetProperty(ref _signSelectIndex, value); }
        }

        public CheckUserModel CheckUserModel
        {
            get { return _checkUserModel; }
            set { SetProperty(ref _checkUserModel, value); }
        }
        //操作界面视标背景
        private SolidColorBrush _backgroundSource;
        //操作界面视标背景
        public SolidColorBrush BackgroundSource
        {
            get => _backgroundSource;
            set => SetProperty(ref _backgroundSource, value);
        }

        private bool _isClicked = false;
        /// <summary>
        /// 是否点击了确认按钮
        /// </summary>
        public bool IsClicked
        {
            get { return _isClicked; }
            set { SetProperty(ref _isClicked, value); }
        }


        private string _signPath;
        /// <summary>
        /// 操作界面显示视标名称
        /// </summary>
        public string SignPath
        {
            get
            {
                return _signPath;
            }
            set
            {
                SetProperty(ref _signPath, value);

                if (string.IsNullOrWhiteSpace(_signPath))
                {
                    ShowSign = Visibility.Collapsed;
                }
                else
                {
                    ShowSign = Visibility.Visible;
                }
                RaisePropertyChanged(nameof(ShowSign));
            }
        }

        private Visibility _showSign = Visibility.Collapsed;
        public Visibility ShowSign
        {
            get { return _showSign; }
            set { SetProperty(ref _showSign, value); }
        }


        private int _checkDuration = AppConst.ContrastDuration;
        /// <summary>
        /// 剩余检查时长
        /// </summary>
        public int CheckDuration
        {
            get { return _checkDuration; }
            set { SetProperty(ref _checkDuration, value); }
        }

        private int _checkTotalDuration;
        /// <summary>
        /// 检查总时长
        /// </summary>
        public int CheckTotalDuration { 
            get { return _checkTotalDuration; }
            set { SetProperty(ref _checkTotalDuration, value); }
        }

        
        private int _waitTotalDuration = AppConst.ContrastWaitDuration;
        /// <summary>
        /// 等待总时长
        /// </summary>
        public int WaitTotalDuration
        {
            get { return _waitTotalDuration; }
            set { SetProperty(ref _waitTotalDuration, value); }
        }


        
        private int _waitDuration = AppConst.ContrastWaitDuration;
        /// <summary>
        /// 剩余等待时长
        /// </summary>
        public int WaitDuration
        {
            get { return _waitDuration; }
            set { SetProperty(ref _waitDuration, value); }
        }

        

        private TrainStatus _trainStatus = TrainStatus.Pending;

        private DCKTime _dcktime;
        public DCKTime DckTime
        {
            get { return _dcktime; }
            set { SetProperty(ref _dcktime, value); }
        }


        /// <summary>
        /// 检查状态
        /// </summary>
        public TrainStatus TrainStatus
        {
            get { return _trainStatus; }
            set { SetProperty(ref _trainStatus, value);
                RaisePropertyChanged(nameof(IsTraining));
                RaisePropertyChanged(nameof(TrainStatusText));
            }
        }


        private bool _isTraining;
        public bool IsTraining
        {
            get
            {
                _isTraining = TrainStatus == TrainStatus.Training;
                return _isTraining;
            }
            set { SetProperty(ref _isTraining, value); }
        }


        private string _trainStatusText;

        public string TrainStatusText
        {
            get
            {
                _trainStatusText = TrainStatus.GetDescription();
                return _trainStatusText;
            }
            set { SetProperty(ref _trainStatusText, value); }
        }


        //视标图片名称
        private string _signPictureName;
        
        private System.Timers.Timer _waittimer;
        private System.Timers.Timer _checktimer;

        private int imageIndex = 0;

        private ArrowButtonStauts _arrowButtonStauts = new ArrowButtonStauts();

        public Dictionary<DCKTime, int> dictResult = new Dictionary<DCKTime, int>();
        

        public ContrastTrainingViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,ILight light
            , ISpeechService speechService, ITrainingAndCheckService trainingAndCheckService)
        {
            secondaryChangeEvent = eventAggregator.GetEvent<SecondaryChangeEvent>();
            contrastSignChangeEvent = eventAggregator.GetEvent<SensitivitySignChangeEvent>();
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.light = light;
            this.speechService = speechService;
            this.trainingAndCheckService = trainingAndCheckService;
        }

        private void JoystickAction(ActionArgs actionArgs)
        {
            SetBtnStyle(actionArgs);
            SignSelectIndex = actionArgs.Index;
            if (actionArgs.Action.Command == JoystickStatus.Confirm)
            {

                dictResult.Add(DckTime, actionArgs.Index);
                StopTrain();

            }

        }
        private void SetBtnStyle(ActionArgs actionArgs)
        {
            switch (actionArgs.Action.Command)
            {
                case JoystickStatus.Front:
                    _ = Task.Run(async () =>
                    {
                        ArrowButtonStauts.UpButtonStatus = true;
                        await Task.Delay(50);
                        ArrowButtonStauts.UpButtonStatus = false;

                    });
                    break;
                case JoystickStatus.Back:
                    _ = Task.Run(async () =>
                    {
                        ArrowButtonStauts.DownButtonStatus = true;
                        await Task.Delay(50);
                        ArrowButtonStauts.DownButtonStatus = false;

                    });
                    break;
                case JoystickStatus.Left:
                    _ = Task.Run(async () =>
                    {
                        ArrowButtonStauts.LeftButtonStatus = true;
                        await Task.Delay(50);
                        ArrowButtonStauts.LeftButtonStatus = false;

                    });
                    break;
                case JoystickStatus.Right:
                    _ = Task.Run(async () =>
                    {
                        ArrowButtonStauts.RightButtonStatus = true;
                        await Task.Delay(50);
                        ArrowButtonStauts.RightButtonStatus = false;

                    });
                    break;
                case JoystickStatus.Confirm:
                    _ = Task.Run(async () =>
                    {
                        ArrowButtonStauts.SaveButtonStatus = true;
                        IsClicked = true;
                        await Task.Delay(50);
                        ArrowButtonStauts.SaveButtonStatus = false;
                        IsClicked = false;

                    });
                    break;
                case JoystickStatus.Trigger:
                    _ = Task.Run(async () =>
                    {
                        ArrowButtonStauts.TriggerButtonStatus = true;
                        await Task.Delay(50);
                        ArrowButtonStauts.TriggerButtonStatus = false;

                    });
                    break;
            }
        }
        
       

        private string GetSignName()
        {
            int randomIndex = Utils.GenerateRandomNumber(1, 5, imageIndex);
            imageIndex = randomIndex;
            return $"df_d{randomIndex}.jpg";
        }
        void RefreshSignImage()
        {
            string imageName = GetSignName();
            string signName = imageName.Replace("_d", "_t");
            contrastSignChangeEvent.Publish(new SensitivityChangeSignOptions() { PicturePath = signName, BackgroundBrush = SignBackGround.Black });
            SignPath= imageName;
        }

        /// <summary>
        /// 刷新视标
        /// </summary>
        public DelegateCommand RefreshCommand => new DelegateCommand(RefreshSignImage);

        private void StartTrain()
        {
            TrainStatus=TrainStatus.Training;
            contrastSignChangeEvent.Publish(new() { PicturePath = "", BackgroundBrush = SignBackGround.White });
            BackgroundSource = Brushes.White;
            SignPath = "";
            eventAggregator.GetEvent<JoystickEvent>().Subscribe(JoystickAction);
            resetWaitTimer();
        }

        //重置timer
        void resetWaitTimer()
        {
            WaitDuration = AppConst.ContrastWaitDuration;
            _waittimer.Start();
        }

        private void StopTrain()
        {
            TrainStatus = TrainStatus.Trained;
            secondaryChangeEvent.Publish(new() { Action = ChangeAction.Idle });
            eventAggregator.GetEvent<JoystickEvent>().Unsubscribe(JoystickAction);
            _waittimer?.Stop();
            _waittimer?.Dispose();
            
            _checktimer?.Stop();
            _checktimer?.Dispose();
            _ = speechService.SpeakAsync("检查结束");

            if(CurrentTrainEnterMode== TrainEnterMode.Normal)
            {
                if (AppData.Instance.DeviceRunMode == DeviceRunMode.NETWORKED)
                {
                    MessageBoxResult messageBoxResult = MessageBoxService.Instance.ShowInfoWithCountDown("检查结束", 10, "提示");
                    if (messageBoxResult == MessageBoxResult.OK)
                    {
                        AppData.Instance.Logout();
                        regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_Menu);
                    }
                }
                else
                {
                    MessageBoxService.Instance.ShowFinishWindow();
                }
            }
            else
            {
                _ = speechService.SpeakAsync("当前检查已经结束");
                ItemOrder itemOrder = trainingAndCheckService.GetCurrentItem();
                CloudContrastResult cloudContrastResult = new CloudContrastResult();
                itemOrder.Result = "检查合格";
                itemOrder.ItemData= cloudContrastResult.ToJson();
                trainingAndCheckService.CompleteCurrentItem();
                MessageBoxResult messageBoxResult = MessageBoxService.Instance.ShowInfoWithCountDown("当前检查已经结束！", 3, "提示");
                
            }

            
        }


        private void CheckTimer_Tick(object? sender, EventArgs e)
        {
            CheckDuration--;
            if (CheckDuration == 0)
            {
                _checktimer.Stop();
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    BackgroundSource = Brushes.Black;
                    SignPath = "";
                //设置对比敏感度视标
                contrastSignChangeEvent.Publish(new SensitivityChangeSignOptions() { PicturePath = "", BackgroundBrush = SignBackGround.Black });
                });
            }
        }
        private void WaitTimer_Tick(object? sender, EventArgs e)
        {
            WaitDuration--;
            if (WaitDuration == 0)
            {
                _waittimer.Stop();
                _checktimer.Start();
                //设置对比敏感度视标
                System.Windows.Application.Current.Dispatcher.Invoke(() => {
                    GetShowPictureName();
                    contrastSignChangeEvent.Publish(new SensitivityChangeSignOptions() { PicturePath = _signPictureName, BackgroundBrush = SignBackGround.Black });
                });
                
                

            }
        }
        private void GetShowPictureName()
        {
            int randomIndex = Utils.GenerateRandomNumber(1, 5, imageIndex);
            imageIndex = randomIndex;
            SignPath = $"df_d{imageIndex}.jpg";
            _signPictureName= $"df_t{imageIndex}.jpg";
            BackgroundSource= Brushes.Black;
        }
        
        public ArrowButtonStauts ArrowButtonStauts
        {
            get { return _arrowButtonStauts; }
            set { SetProperty(ref _arrowButtonStauts, value); }
        }

       

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _waittimer = new System.Timers.Timer(1000);
            _waittimer.Elapsed += WaitTimer_Tick;
            _checktimer = new System.Timers.Timer(1000);
            _checktimer.Elapsed += CheckTimer_Tick;
            contrastConfigParam = navigationContext.Parameters.GetValue<ContrastConfigParam>(nameof(ContrastConfigParam));
            DckTime = contrastConfigParam.CheckDuration;
            CheckTotalDuration = CheckDuration = int.Parse(DckTime.GetDescription());
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Contrast });
            CheckUserModel = new CheckUserModel()
            {
                Id = AppData.Instance.CurrentPatient.Id,
                Name = AppData.Instance.CurrentPatient.PatientName,
                Gender = AppData.Instance.CurrentPatient.Gender,
                Age = AppData.Instance.CurrentPatient.Age.ToString(),
                PD= contrastConfigParam.PD,
            };
            dictResult.Clear();
            light.TurnOnAll();
            StartTrain();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            StopTrain();
             light.TurnOffAll();
            eventAggregator.GetEvent<JoystickEvent>().Unsubscribe(JoystickAction);
        }
    }
}
