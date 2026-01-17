using Dm;
using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using NetTaste;
using Prism.Events;
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
    [RegionMemberLifetime(KeepAlive = false)]
    public class ContrastTrainingViewModel : BaseViewModel, INavigationAware
    {
        private readonly SecondaryChangeEvent secondaryChangeEvent;//副屏页面切换事件
        private readonly SensitivitySignChangeEvent contrastSignChangeEvent;//对比度标志位事件
        private readonly IEventAggregator eventAggregator;
        private readonly ILight light;
        private CheckUserModel _checkUserModel = new CheckUserModel();

        ContrastConfigParam contrastConfigParam;
        //选择视标的索引
        private int _signSelectIndex = 0;
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


        private int _checkDuration = 5;
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
            set { SetProperty(ref _trainStatus, value); }
        }

        private string _trainStatusDisplay;

        public string TrainStatusDisplay
        {
            get
            {
                _trainStatusDisplay = dictTrainDisplay[TrainStatus];
                return _trainStatusDisplay;
            }
            set { SetProperty(ref _trainStatusDisplay, value); }
        }


        //视标图片名称
        private string _signPictureName;
        
        private System.Timers.Timer _waittimer;
        private System.Timers.Timer _checktimer;

        private int imageIndex = 0;

        private ArrowButtonStauts _arrowButtonStauts = new ArrowButtonStauts();

        public Dictionary<DCKTime, int> dictResult = new Dictionary<DCKTime, int>();
        public Dictionary<TrainStatus,string> dictTrainDisplay= new Dictionary<TrainStatus, string>()
        {
            { TrainStatus.Pending,"等待检查" },
            { TrainStatus.Training,"正在检查" },
            { TrainStatus.Trained,"检查完成" },
            { TrainStatus.NotTrain,"无需检查" },
        };

        public ContrastTrainingViewModel(IRegionManager regionManager, IEventAggregator eventAggregator,ILight light)
        {
            secondaryChangeEvent = eventAggregator.GetEvent<SecondaryChangeEvent>();
            contrastSignChangeEvent = eventAggregator.GetEvent<SensitivitySignChangeEvent>();
            
            _waittimer = new System.Timers.Timer(1000);
            _waittimer.Elapsed += WaitTimer_Tick;
            _checktimer = new System.Timers.Timer(1000);
            _checktimer.Elapsed += CheckTimer_Tick;
            this.eventAggregator = eventAggregator;
            this.light = light;
        }

        private void JoystickAction(ActionArgs actionArgs)
        {
            SetBtnStyle(actionArgs);
            SignSelectIndex = actionArgs.Index;
            if (actionArgs.Action.Command == JoystickStatus.Confirm)
            {
                //记录va值
                if (DckTime == DCKTime.T50)
                {
                    StopTrain();
                }
                else
                {
                    dictResult.Add(DckTime, actionArgs.Index);
                    DckTime = DckTime.Next();
                    RefreshSignImage();
                    StartTrain();
                }

                
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
                        await Task.Delay(50);
                        ArrowButtonStauts.SaveButtonStatus = false;

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
            secondaryChangeEvent.Publish(new() { Action = ChangeAction.Idle });
            _waittimer?.Stop();
            _waittimer?.Dispose();
            _checktimer?.Stop();
            _checktimer?.Dispose();

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
