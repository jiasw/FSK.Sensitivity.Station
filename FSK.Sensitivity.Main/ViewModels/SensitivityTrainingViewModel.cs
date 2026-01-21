using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Core.Utility;
using FSK.Sensitivity.Main.Controls;
using Newtonsoft.Json.Linq;
using Prism.Navigation.Regions;
using SqlSugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class CSFTrainModel : BaseViewModel,IDisposable
    {
        private string title = "";
        public string Title
        {   
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        private TrainStatus trainStatus = TrainStatus.Pending;

        public TrainStatus TrainStatus { 
            get { return trainStatus; }
            set {
                SetProperty(ref trainStatus, value);
                RaisePropertyChanged(nameof(IsTraining));
                RaisePropertyChanged(nameof(TrainStatusText));
            }
        }
                
                

        private string _trainStatusText;

        public string TrainStatusText
        {

            get
            {
                switch (TrainStatus)
                {
                    case TrainStatus.Pending:
                        _trainStatusText= "等待检查";
                        break;
                    case TrainStatus.Training:
                        _trainStatusText= "正在检查";break;
                    case TrainStatus.Trained:
                        _trainStatusText= "检查完成";break;
                    case TrainStatus.NotTrain:
                        _trainStatusText= "无需检查"; break;
                    default:
                        _trainStatusText= ""; break;
                }
                return _trainStatusText;
            }
            set { SetProperty(ref _trainStatusText, value); }
        }
        private string vavaluetext;
        public string VAValueText { 
        
            get {  vavaluetext= VA.GetDescription(); return vavaluetext; }
            set
            {
                SetProperty(ref vavaluetext, value);
            }
            }

        public string DistanceText { get; set; } = "";

        private bool _isTraining;
        public bool IsTraining
        {
            get
            {
                _isTraining= TrainStatus == TrainStatus.Training;
                return _isTraining;
            }
            set { SetProperty(ref _isTraining, value); }
        }

        public string AT { get; set; } = "0";

        private int rt = 0;
        public int RT
        {
            get { return rt; }
            set {
                
                SetProperty(ref rt, value);
                if (rt<=0)
                {
                    timer.Stop();
                    TrainStatus = TrainStatus.Trained;
                    TrainFinish.Invoke(this, new EventArgs());
                }
            }
        }

        private CSFVA va = CSFVA.VA06;
        public CSFVA VA
        {
            get { return va; }
            set { SetProperty(ref va, value);
                RaisePropertyChanged(nameof(VAValueText));
            
            }

        }

        
        private System.Timers.Timer timer = new System.Timers.Timer(1000);
        public void StartTrain()
        {
            DictResult.Clear();
            timer.Start();
            TrainStatus = TrainStatus.Training;
            timer.Elapsed += Timer_Elapsed;
            this.IsTraining = true;
            
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (RT>0)
            {
                RT--;
            }
            if (RT <= 0)
            {
                TrainFinish?.Invoke(this, new EventArgs());
                StopTrain();
            }
            
        }
        

        public void StopTrain()
        {
            timer.Stop();
            TrainFinish?.Invoke(this, new EventArgs());

        }

        public void Dispose()
        {
            timer?.Stop();
            timer?.Dispose();
        }

        /// <summary>
        /// 本次训练完成事件
        /// </summary>
        public event EventHandler TrainFinish;

        public Dictionary<CSFVA, int> DictResult = new Dictionary<CSFVA, int>();

    }
    
    public class SensitivityTrainingViewModel:BaseViewModel,INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly CheckResultRepository checkResultRepository;
        private readonly IMotor motor;
        private readonly IRegionNavigationJournal journal;
        private readonly SecondaryChangeEvent secondaryChangeEvent;
        private readonly SensitivitySignChangeEvent sensitivitySignChangeEvent;
        
        private CSFTrainModel leftCSFTrainModel = new CSFTrainModel();
        private CSFTrainModel rightCSFTrainModel = new CSFTrainModel();
        private CSFTrainModel currentCSFTrainModel = null;
        private long checkid = -1;//当前检查的ID，
        private CancellationTokenSource _cts;
        //选择视标的索引
        private int _signSelectIndex = 0;
        public int SignSelectIndex
        {
            get { return _signSelectIndex; }
            set { SetProperty(ref _signSelectIndex, value); }
        }

        public SensitivityTrainingViewModel(IRegionManager regionManager,IEventAggregator eventAggregator, CheckResultRepository checkResultRepository, IMotor motor)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.checkResultRepository = checkResultRepository;
            this.motor = motor;
            this.journal = regionManager.Regions[AppConst.MainRegion].NavigationService.Journal;
            secondaryChangeEvent = eventAggregator.GetEvent<SecondaryChangeEvent>();
            sensitivitySignChangeEvent= eventAggregator.GetEvent<SensitivitySignChangeEvent>();
        }

        

        public CSFTrainModel LeftCSFTrainModel
        {
            get { return leftCSFTrainModel; }
            set { SetProperty(ref leftCSFTrainModel, value); }
        }

        public CSFTrainModel RightCSFTrainModel
        {
            get { return rightCSFTrainModel; }
            set { SetProperty(ref rightCSFTrainModel, value); }
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



        private Queue<CSFTrainModel> TrainModelsQueue = new Queue<CSFTrainModel>();


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
            string signName = "t" + imageName.Substring(1);
            SignBackGround signBackGround = CheckUserModel.DayNight==Core.Enums.DayOrNight.Day? SignBackGround.White : SignBackGround.Black;
            
           Application.Current.Dispatcher.Invoke(() =>
            {
                sensitivitySignChangeEvent.Publish(new SensitivityChangeSignOptions() { PicturePath = signName, BackgroundBrush = signBackGround });
                SignPath = imageName;
            });
        }
        private string GetSignName()
        {
            
            string strdaycode = "d";
            if (CheckUserModel.DayNight == Core.Enums.DayOrNight.Night)
            {
                strdaycode = "k";
            }
            Dictionary<CSFVA, string> dicVA = new Dictionary<CSFVA, string>()
            {
                {CSFVA.VA06, "06" },
                {CSFVA.VA10, "1" },
                {CSFVA.VA20, "2" },
                {CSFVA.VA40, "4" },
                {CSFVA.VA60, "6" },
                {CSFVA.VA80, "8" },
            };
            int randomIndex = Utils.GenerateRandomNumber(1, 5, imageIndex);
            imageIndex = randomIndex;
            return $"d-{strdaycode}-{dicVA[currentCSFTrainModel.VA]}-{randomIndex}.jpg";
        }

        


        /// <summary>
        /// 用户选择视标事件
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void VaValueChanged(int obj)
        {
            //记录va值
            Trace.WriteLine($"VA Value Changed:{obj}, vaValue:{currentCSFTrainModel.VA}");
            currentCSFTrainModel.DictResult.Add(currentCSFTrainModel.VA,obj);

            if (currentCSFTrainModel.VA == CSFVA.VA80)
            {
                currentCSFTrainModel.StopTrain();
            }
            else
            {
                currentCSFTrainModel.VA = currentCSFTrainModel.VA.Next();
                RefreshSignImage();
            }
            
        }

        
        private void JoystickAction(ActionArgs actionArgs)
        {
            if(currentCSFTrainModel==null || !currentCSFTrainModel.IsTraining)
            {
                return;
            }
            SetBtnStyle(actionArgs);
            if (actionArgs.Action.Command== JoystickStatus.Confirm)
            {
                VaValueChanged(actionArgs.Index);
            }
            
        }

        

        private void SetBtnStyle(ActionArgs actionArgs)
        {
            SignSelectIndex=actionArgs.Index;

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


        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CheckUserModel model = new CheckUserModel()
            {
                Id = AppData.Instance.CurrentPatient.Id,
                Name = AppData.Instance.CurrentPatient.PatientName,
                Gender = AppData.Instance.CurrentPatient.Gender,
                Age = AppData.Instance.CurrentPatient.Age.ToString(),
               
            };
            currentCSFTrainModel = null;
            checkid=Utils.GenerateSnowID();
            TrainModelsQueue.Clear();
            
            SensitivityConfigParam? sensitivityConfigParam = navigationContext.Parameters[nameof(SensitivityConfigParam)] as SensitivityConfigParam;
            if (sensitivityConfigParam != null)
            {
                model.PD = sensitivityConfigParam.PD;
                model.Light = sensitivityConfigParam.IsLightOn ;
                model.DayNight = sensitivityConfigParam.DayNight;
                model.CheckEyeMode = sensitivityConfigParam.Eyes;
                model.checkDistance = sensitivityConfigParam.CheckDistance;
                if (sensitivityConfigParam.Eyes== Eye.OS)
                {
                    LeftCSFTrainModel = new CSFTrainModel()
                    {
                        AT = AppConst.LeftEyeDuration.ToString(),
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VA=CSFVA.VA06,
                        RT = AppConst.LeftEyeDuration,
                        TrainStatus = TrainStatus.Pending
                    };
                    RightCSFTrainModel = new CSFTrainModel()
                    {
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VA = CSFVA.VA06,
                        TrainStatus = TrainStatus.NotTrain
                    };
                } 
                else if(sensitivityConfigParam.Eyes == Eye.OD)
                {
                    RightCSFTrainModel = new CSFTrainModel()
                    {
                        AT = AppConst.LeftEyeDuration.ToString(),
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VA = CSFVA.VA06,
                        RT = AppConst.LeftEyeDuration,
                        TrainStatus = TrainStatus.Pending
                    };
                    LeftCSFTrainModel = new CSFTrainModel()
                    {
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VA = CSFVA.VA06,
                        TrainStatus = TrainStatus.NotTrain
                    };
                }
                else
                {
                    
                    RightCSFTrainModel = new CSFTrainModel()
                    {
                        AT = AppConst.LeftEyeDuration.ToString(),
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VA = CSFVA.VA06,
                        RT = AppConst.LeftEyeDuration,
                        TrainStatus = TrainStatus.Pending
                    };
                    LeftCSFTrainModel = new CSFTrainModel()
                    {
                        AT = AppConst.LeftEyeDuration.ToString(),
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VA = CSFVA.VA06,
                        RT = AppConst.LeftEyeDuration,
                        TrainStatus = TrainStatus.Pending,
                    };
                }
                
            }
            TrainModelsQueue.Enqueue(LeftCSFTrainModel);
            TrainModelsQueue.Enqueue(RightCSFTrainModel);

            CheckUserModel = model;
            //加载对比敏感度视标页面
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Sensitivity, Brush = CheckUserModel.DayNight == Core.Enums.DayOrNight.Day ? SignBackGround.White : SignBackGround.Black });
           _= StartTask();
            
            eventAggregator.GetEvent<JoystickEvent>().Subscribe(JoystickAction);
        }
        private async Task SaveTrainResult()
        {
            if(LeftCSFTrainModel.TrainStatus== TrainStatus.Trained)
            {
                await checkResultRepository.Add(new Core.Entity.CheckResult()
                {
                    CheckId = checkid,
                    DayOrNight = CheckUserModel.DayNight,
                    UserId = CheckUserModel.Id,
                    FitTime = DateTime.Now,
                    CheckDuration = CheckUserModel.CurrentDuration,
                    CheckDistance = CheckUserModel.checkDistance,
                    LightStatus = CheckUserModel.Light,
                    Eye = Eye.OS,
                    PupilDistance = CheckUserModel.PD,
                    VA06 = GetVaResult(LeftCSFTrainModel.DictResult, CSFVA.VA06),
                    VA10 = GetVaResult(LeftCSFTrainModel.DictResult, CSFVA.VA10),
                    VA20 = GetVaResult(LeftCSFTrainModel.DictResult, CSFVA.VA20),
                    VA40 = GetVaResult(LeftCSFTrainModel.DictResult, CSFVA.VA40),
                    VA60 = GetVaResult(LeftCSFTrainModel.DictResult, CSFVA.VA60),
                    VA80 = GetVaResult(LeftCSFTrainModel.DictResult, CSFVA.VA80),
                });
            }
            if (RightCSFTrainModel.TrainStatus==TrainStatus.Trained)
            {
                await checkResultRepository.Add(new Core.Entity.CheckResult()
                {
                    CheckId = checkid,
                    DayOrNight = CheckUserModel.DayNight,
                    UserId = CheckUserModel.Id,
                    FitTime = DateTime.Now,
                    CheckDuration = CheckUserModel.CurrentDuration,
                    CheckDistance = CheckUserModel.checkDistance,
                    LightStatus = CheckUserModel.Light,
                    Eye = Eye.OD,
                    PupilDistance = CheckUserModel.PD,
                    VA06 = GetVaResult(RightCSFTrainModel.DictResult, CSFVA.VA06),
                    VA10 = GetVaResult(RightCSFTrainModel.DictResult, CSFVA.VA10),
                    VA20 = GetVaResult(RightCSFTrainModel.DictResult, CSFVA.VA20),
                    VA40 = GetVaResult(RightCSFTrainModel.DictResult, CSFVA.VA40),
                    VA60 = GetVaResult(RightCSFTrainModel.DictResult, CSFVA.VA60),
                    VA80 = GetVaResult(RightCSFTrainModel.DictResult, CSFVA.VA80),
                });
            }

            
        }

        int GetVaResult(Dictionary<CSFVA, int> result, CSFVA va)
        {
            if (result.ContainsKey(va))
            {
                return result[va];
            }
            else
            {
                return 0;
            }
        }



        private bool _isProcessing = false;
        private async Task StartTask()
        {
            if (_isProcessing) return;
            _isProcessing = true;
            // 每次启动时创建一个新的 TokenSource
            _cts = new CancellationTokenSource();
            
            var token = _cts.Token;
            while (TrainModelsQueue.Count > 0)
            {
                // 1. 在循环开始处检查取消请求
                if (token.IsCancellationRequested) break;
                currentCSFTrainModel = TrainModelsQueue.Dequeue();
                RefreshSignImage();

                if (currentCSFTrainModel.TrainStatus == TrainStatus.Pending)
                {

                    // 创建一个可以等待的任务
                    var tcs = new TaskCompletionSource<bool>();
                    // 2. 注册取消回调：如果用户点击取消，立即结束等待
                    using (token.Register(() => tcs.TrySetCanceled()))
                    {
                        EventHandler handler = (s, e) => tcs.TrySetResult(true);
                        currentCSFTrainModel.TrainFinish += handler;

                        try
                        {
                            currentCSFTrainModel.StartTrain();

                            await tcs.Task; 
                        }
                        finally
                        {
                            currentCSFTrainModel.TrainFinish -= handler;
                        }
                    }
                }
            }

            // 循环结束后执行清理逻辑
            await SaveTrainResult();
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Idle });
            eventAggregator.GetEvent<JoystickEvent>().Unsubscribe(JoystickAction);
            await motor.StopAllMotor();
            if (TrainModelsQueue.Count <= 0)
            {
                MessageBoxService.Instance.Show("检查结束");
            }
            _isProcessing = false;
        }

        

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _cts?.Cancel();
            currentCSFTrainModel?.StopTrain();
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Idle });
            eventAggregator.GetEvent<JoystickEvent>().Unsubscribe(JoystickAction);
             motor.StopAllMotor();
        }

        public DelegateCommand ListResultCommand => new DelegateCommand(ListResult);

        private void ListResult()
        {
            // TODO: List the result of the test
        }
    }
}
