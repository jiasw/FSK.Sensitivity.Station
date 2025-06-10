using FSK.Sensitivity.Core;
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


    public class CSFTrainModel : BaseViewModel
    {
        private string title = "";
        public string Title
        {   
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        public TrainStatus TrainStatus { get; set; } = TrainStatus.Pending;

        public string TrainStatusText
        {

            get
            {
                switch (TrainStatus)
                {
                    case TrainStatus.Pending:
                        return "等待训练";
                    case TrainStatus.Training:
                        return "正在训练";
                    case TrainStatus.Trained:
                        return "训练完成";
                    case TrainStatus.NotTrain:
                        return "无需训练";
                    default:
                        return "";
                }
            }
        }

        public string VAValueText { get; set; } = "";

        public string DistanceText { get; set; } = "";


        public bool IsTraining
        {
            get
            {
                return TrainStatus == TrainStatus.Training;
            }
        }

        public string AT { get; set; } = "0";

        private int rt = 0;
        public int RT
        {
            get { return rt; }
            set { SetProperty(ref rt, value); }
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
        private VAValue _vaValue = VAValue.VA06;
        private Eye CurrentCheckEye = Eye.OS;
        private Eye CheckPlan = Eye.OS;//检查双眼时，先检查左眼，再检查右眼
        private int CheckDruationTime = 30;//每只眼睛检查时间
        private CSFTrainModel leftCSFTrainModel = new CSFTrainModel();
        private CSFTrainModel rightCSFTrainModel = new CSFTrainModel();
        



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
            SignBackGround signBackGround = CheckUserModel.DayNight==Core.Enums.DayOrNight.Day? SignBackGround.White : SignBackGround.Black;
            sensitivitySignChangeEvent.Publish(new SensitivityChangeSignOptions() { PicturePath = signName,BackgroundBrush= signBackGround });
            SignImage = bitmapImage;
        }
        private string GetSignName()
        {
            
            string strdaycode = "d";
            if (CheckUserModel.DayNight == Core.Enums.DayOrNight.Night)
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

        private async Task StartTrain()
        {

            while (LeftCSFTrainModel.RT > 0)
            {
                LeftCSFTrainModel.TrainStatus= TrainStatus.Training;
                await Task.Delay(1000);
                LeftCSFTrainModel.RT--;
            }
            LeftCSFTrainModel.TrainStatus = TrainStatus.Trained;
            while (RightCSFTrainModel.RT > 0)
            {
                RightCSFTrainModel.TrainStatus = TrainStatus.Training;
                await Task.Delay(1000);
                RightCSFTrainModel.RT--;
            }
            RightCSFTrainModel.TrainStatus = TrainStatus.Trained;
            

        }





        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _vaValue = VAValue.VA06;
            CurrentCheckEye = Eye.OS;
            CheckPlan = Eye.OS;//检查双眼时，先检查左眼，再检查右眼
            CheckDruationTime = 30;//每只眼睛检查时间
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
                model.Light = sensitivityConfigParam.IsLightOnDisplay ? "开" : "关";
                model.DayNight = sensitivityConfigParam.DayNight;

                if(sensitivityConfigParam.Eyes== Eye.OS)
                {
                    LeftCSFTrainModel = new CSFTrainModel()
                    {
                        AT = AppConst.LeftEyeDruation.ToString(),
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VAValueText = _vaValue.GetDescription(),
                        RT = AppConst.LeftEyeDruation,
                        TrainStatus = TrainStatus.Pending
                    };
                    rightCSFTrainModel = new CSFTrainModel()
                    {
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VAValueText = _vaValue.GetDescription(),
                        TrainStatus = TrainStatus.NotTrain
                    };
                } 
                else if(sensitivityConfigParam.Eyes == Eye.OD)
                {
                    rightCSFTrainModel = new CSFTrainModel()
                    {
                        AT = AppConst.LeftEyeDruation.ToString(),
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VAValueText = _vaValue.GetDescription(),
                        RT = AppConst.LeftEyeDruation,
                        TrainStatus = TrainStatus.Pending
                    };
                    LeftCSFTrainModel = new CSFTrainModel()
                    {
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VAValueText = _vaValue.GetDescription(),
                        TrainStatus = TrainStatus.NotTrain
                    };
                }
                else
                {
                    LeftCSFTrainModel = new CSFTrainModel()
                    {
                        AT = AppConst.LeftEyeDruation.ToString(),
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VAValueText = _vaValue.GetDescription(),
                        RT = AppConst.LeftEyeDruation,
                        TrainStatus = TrainStatus.Pending
                    };
                    rightCSFTrainModel = new CSFTrainModel()
                    {
                        AT = AppConst.LeftEyeDruation.ToString(),
                        DistanceText = sensitivityConfigParam.CheckDistanceDisplay,
                        VAValueText = _vaValue.GetDescription(),
                        RT = AppConst.LeftEyeDruation,
                        TrainStatus = TrainStatus.Pending
                    };
                }
                
            }
           

       CheckUserModel = model;
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Sensitivity, Brush = CheckUserModel.DayNight == Core.Enums.DayOrNight.Day ? SignBackGround.White : SignBackGround.Black });
            RefreshSignImage();
            Task.Run(async () =>
            {
                await StartTrain();
            });
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
