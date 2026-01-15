using Dm;
using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.Utility;
using NetTaste;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        private CheckUserModel _checkUserModel = new CheckUserModel();
        public CheckUserModel CheckUserModel
        {
            get { return _checkUserModel; }
            set { SetProperty(ref _checkUserModel, value); }
        }
        //操作界面视标图片
        private object _backgroundSource;
        //操作界面视标图片
        public object BackgroundSource
        {
            get => _backgroundSource;
            set => SetProperty(ref _backgroundSource, value);
        }
        //检查时长（秒）
        private int _checkDuration = 5;
        public int CheckDuration
        {
            get { return _checkDuration; }
            set { SetProperty(ref _checkDuration, value); }
        }

        private int _checkDurationDisplay;
        /// <summary>
        /// 检查时长显示
        /// </summary>
        public int CheckDurationDisplay { 
            get { return _checkDurationDisplay; }
            set { SetProperty(ref _checkDurationDisplay, value); }
        }

        
        private int _countdown;
        /// <summary>
        /// 倒计时显示
        /// </summary>
        public int CountdownDisplay
        {
            get { return _countdown; }
            set { SetProperty(ref _countdown, value); }
        }


        //等待时长（秒）
        private int _waitDuration = AppConst.ContrastWaitDuration;
        public int WaitDuration
        {
            get { return _waitDuration; }
            set { SetProperty(ref _waitDuration, value); }
        }

        private int _at;
        public int At
        {
            get { return _at; }
            set { SetProperty(ref _at, value); }
        }

        private int _rt;
        public int RT
        {
            get { return _rt; }
            set { SetProperty(ref _rt, value); }
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
        
        private DispatcherTimer _waittimer;
        private DispatcherTimer _checktimer;

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

        public ContrastTrainingViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            secondaryChangeEvent = eventAggregator.GetEvent<SecondaryChangeEvent>();
            contrastSignChangeEvent = eventAggregator.GetEvent<SensitivitySignChangeEvent>();
            
            _waittimer = new DispatcherTimer();
            _waittimer.Interval = TimeSpan.FromSeconds(1);
            _waittimer.Tick += WaitTimer_Tick;
            _checktimer = new DispatcherTimer();
            _checktimer.Interval = TimeSpan.FromSeconds(1);
            _checktimer.Tick += CheckTimer_Tick;
            this.eventAggregator = eventAggregator;
        }

        private void JoystickAction(ActionArgs actionArgs)
        {
            SetBtnStyle(actionArgs);
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
            BackgroundSource = imageName;
        }


        private void StartTrain()
        {
            
            contrastSignChangeEvent.Publish(new() { PicturePath = "", BackgroundBrush = SignBackGround.White });
            BackgroundSource = SignBackGround.White;
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
            _waittimer.Stop();
            _checktimer.Stop();

        }


        private void CheckTimer_Tick(object? sender, EventArgs e)
        {
            CheckDuration--;
            if (CheckDuration == 0)
            {
                _checktimer.Stop();
                BackgroundSource = SignBackGround.Black;
                //设置对比敏感度视标
                contrastSignChangeEvent.Publish(new SensitivityChangeSignOptions() { PicturePath = "", BackgroundBrush = SignBackGround.Black });
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
                GetShowPictureName();
                contrastSignChangeEvent.Publish(new SensitivityChangeSignOptions() { PicturePath = _signPictureName, BackgroundBrush = SignBackGround.Black });

            }
        }
        private void GetShowPictureName()
        {
            int randomIndex = Utils.GenerateRandomNumber(1, 5, imageIndex);
            imageIndex = randomIndex;
            BackgroundSource = $"df_d{imageIndex}.jpg";
            _signPictureName= $"df_t{imageIndex}.jpg";
        }



        

        



        
        public ArrowButtonStauts ArrowButtonStauts
        {
            get { return _arrowButtonStauts; }
            set { SetProperty(ref _arrowButtonStauts, value); }
        }

        DCKTime GetDckTime(int time)
        {
            Dictionary<int,DCKTime> dict= new Dictionary<int, DCKTime>()
            {
                {5,DCKTime.T05 },
                {10,DCKTime.T10 },
                {15,DCKTime.T15 },
                {20,DCKTime.T20 },
                {30,DCKTime.T30 },
                {50,DCKTime.T50 },
            };
            if (dict.ContainsKey(time))
            {
                return dict[time];
            }
            else
            {
                return DCKTime.T05;
            }
        }


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CheckDuration = navigationContext.Parameters.GetValue<int>("time");
            DckTime= GetDckTime(CheckDuration);
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Contrast });
            CheckUserModel = new CheckUserModel()
            {
                Id = AppData.Instance.CurrentPatient.Id,
                Name = AppData.Instance.CurrentPatient.PatientName,
                Gender = AppData.Instance.CurrentPatient.Gender,
                Age = AppData.Instance.CurrentPatient.Age.ToString(),
                
            };
            dictResult.Clear();
            StartTrain();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            StopTrain();
        }
    }
}
