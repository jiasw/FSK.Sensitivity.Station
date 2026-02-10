using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Core.Utility;
using FSK.Sensitivity.Main.Controls;
using HandyControl.Controls;
using Microsoft.Extensions.Logging;
using Prism.Dialogs;
using Prism.Events;
using Prism.Navigation.Regions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Navigation;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class MainMenuViewModel : BaseViewModel,IActiveAware, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly IModbusService modbusService;
        private readonly IConfigurationService configurationService;
        private readonly ISecureRegistrationService secureRegistrationService;
        private readonly ICloudSyncService cloudSyncService;
        private readonly ILogger<MainMenuViewModel> logger;
        private readonly PatientRepository patientRepository;
        private readonly MangerRepository mangerRepository;
        private readonly ITrainingAndCheckService trainingAndCheckService;
        private readonly ISpeechService speechService;
        private readonly IHeartbeatService heartbeatService;
        private readonly IMotor motor;
        private AppSetting appSetting;
        private System.Timers.Timer checkNetWorkTimer;

        public MainMenuViewModel(IRegionManager regionManager, IDialogService dialogService
            , IModbusService modbusService, IConfigurationService configurationService
            , ISecureRegistrationService secureRegistrationService, ICloudSyncService cloudSyncService
            ,ILogger<MainMenuViewModel> logger, PatientRepository patientRepository, MangerRepository mangerRepository
            , ITrainingAndCheckService trainingAndCheckService,IEventAggregator eventAggregator
            , ISpeechService speechService, IHeartbeatService heartbeatService, IMotor motor)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.modbusService = modbusService;
            this.configurationService = configurationService;
            this.secureRegistrationService = secureRegistrationService;
            this.cloudSyncService = cloudSyncService;
            this.logger = logger;
            this.patientRepository = patientRepository;
            this.mangerRepository = mangerRepository;
            this.trainingAndCheckService = trainingAndCheckService;
            this.speechService = speechService;
            this.heartbeatService = heartbeatService;
            this.motor = motor;
            trainingAndCheckService.TrainingItemStarted += TrainingAndCheckService_TrainingItemStarted;
            trainingAndCheckService.TrainingItemCompleted += TrainingAndCheckService_TrainingItemCompleted;
            trainingAndCheckService.TrainingFinished += TrainingAndCheckService_TrainingFinished;

            checkNetWorkTimer = new System.Timers.Timer(10000); // 设置定时器间隔为5秒
            checkNetWorkTimer.Elapsed += new ElapsedEventHandler(CheckNetWork);
            checkNetWorkTimer.Start();

            
        }

        private void OnCloudTrainAndCheck(TrainAndCheckOptions trainAndCheckOptions)
        {

        }

        private void CheckNetWork(object? sender, ElapsedEventArgs e)
        {
            NetWorkActive=Utils.CheckInternalNetWorkStatus();
            
        }

        private string _title = "主菜单";

        public event EventHandler IsActiveChanged;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private Visibility _loginVisibility = Visibility.Visible;
        public Visibility LoginVisibility
        {   
            get { return _loginVisibility; }
            set { SetProperty(ref _loginVisibility, value); }
        }

        private Visibility _userVisibility = Visibility.Collapsed;
        public Visibility UserVisibility
        {
            get { return _userVisibility; }
            set { SetProperty(ref _userVisibility, value);
                if (value == Visibility.Visible) { 
                LoginVisibility = Visibility.Collapsed;
                    }
                else
                {
                    LoginVisibility = Visibility.Visible;
                }
            
            }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        /// <summary>
        /// 网络连接状态
        /// </summary>
        private bool _netWorkActive = false;
        public bool NetWorkActive
        {
            get { return _netWorkActive; }
            set { SetProperty(ref _netWorkActive, value); }
        }
        /// <summary>
        /// 工作台连接状态
        /// </summary>
        private bool _platformActive = false;
        public bool PlatformActive
        {
            get { return _platformActive; }
            set { SetProperty(ref _platformActive, value); }
        }


        public DelegateCommand LoadedCommand => new DelegateCommand( async () => await Loaded());

        private async Task Loaded()
        {
            this.appSetting = configurationService.LoadSetting();
            AppData.Instance.DeviceRunMode = appSetting.DeviceRunMode;
            await CheckDeviceStatus();

        }


        // 检查设备状态
        private async Task<bool> CheckDeviceStatus()
        {
            IsLoading = true;
            //检查设备硬件状态
            if (!modbusService.IsConnected)
            {
                if (string.IsNullOrWhiteSpace(appSetting.ModbusPortName))
                {
                    IsLoading = false;
                    MessageBoxService.Instance.Show("请先设置硬件设备连接端口！", "设备未连接", MessageBoxButton.OK);
                    return false;
                }
                modbusService.Initialize(appSetting.ModbusPortName);
                if (!modbusService.Connect())
                {
                    IsLoading = false;
                    MessageBoxService.Instance.Show("硬件设备未连接，请连接后重试！", "设备未连接", MessageBoxButton.OK);
                    return false;
                }
            }
            
            if (!await InitializeDeviceStatusAsync())
            {
                IsLoading = false;
                return false;
            }
            IsLoading = false;
            return true;
        }

        public DelegateCommand ExitCommand=> new DelegateCommand(ExitLogin);

        private void ExitLogin()
        {
            AppData.Instance.CurrentPatient = null;
            AppData.Instance.IsLogin = false;
            UserVisibility = Visibility.Collapsed;
            LoginVisibility = Visibility.Visible;
        }

       

        private async Task<bool> InitializeDeviceStatusAsync()
        {
            /*
             1.判断设备能否连上服务器
            2.判断设备是否注册
            3.判断设备是否激活
            4.判断硬件设备是否连接
             
             */
            
            if (!AppData.Instance.IsConnectCloud)
            {
                LoadingMessageText = "正在连接服务器...";
                bool connectCloud= await cloudSyncService.IsCanConnect();
                if (!connectCloud)
                {
                    MessageBoxService.Instance.Show("无法连接服务器，请稍后重试", "连接服务器失败", MessageBoxButton.OK);
                    return false;
                }
                else
                {
                    AppData.Instance.IsConnectCloud = true;
                }
            }
            PlatformActive = AppData.Instance.IsConnectCloud;

            if (!AppData.Instance.IsRegister)
            {
                LoadingMessageText = "正在检查设备注册信息...";
                /*
                 判断本机是否有注册信息
                 */
                DeviceRegisterModel? deviceRegisterModel = secureRegistrationService.LoadRegistration();
                if (deviceRegisterModel == null) {
                    //判断是否有设备类型
                    if(string.IsNullOrWhiteSpace(appSetting.DeviceInfo.DeviceType))
                    {
                        MessageBoxService.Instance.Show("请联系工程师配置设备类型信息！", "注册", MessageBoxButton.OK);
                        return false;
                    }
                    if (string.IsNullOrWhiteSpace(appSetting.DeviceInfo.DeviceModel))
                    {
                        MessageBoxService.Instance.Show("请联系工程师配置设备型号信息！", "注册", MessageBoxButton.OK);
                        return false;
                    }
                    //弹窗提示是否注册设备
                    if (MessageBoxService.Instance.ShowConfirm("请确认是否注册设备？", "注册") == MessageBoxResult.No)
                    {
                        return false;
                    }

                    //执行注册逻辑
                    DeviceRegistResult registResult= await cloudSyncService.RegisterDevice();
                    if (!registResult.Result)
                    {
                        MessageBoxService.Instance.Show("设备注册失败,请稍候重试", "注册", MessageBoxButton.OK);
                        return false;
                    }
                    else
                    {
                        if (string.IsNullOrWhiteSpace(appSetting.DeviceInfo.DeviceNo) )
                        {
                            appSetting.DeviceInfo.DeviceNo=registResult.DeviceNum;
                            configurationService.SaveSetting(appSetting);
                        }
                        AppData.Instance.DeviceNo = registResult.DeviceNum;
                        secureRegistrationService.SaveRegistration(new DeviceRegisterModel() { 
                            DeviceNo=registResult.DeviceNum,
                        });
                        AppData.Instance.IsRegister = true;
                    }
                }
                else
                {
                    if (appSetting.DeviceInfo.DeviceNo != deviceRegisterModel.DeviceNo)
                    {
                        appSetting.DeviceInfo.DeviceNo = deviceRegisterModel.DeviceNo;
                        configurationService.SaveSetting(appSetting);
                        this.appSetting = configurationService.LoadSetting();
                    }
                    AppData.Instance.DeviceNo = deviceRegisterModel.DeviceNo;
                    AppData.Instance.IsRegister = true;
                }
            }
            if (!string.IsNullOrWhiteSpace(AppData.Instance.DeviceNo))
            {
                heartbeatService.Start(AppData.Instance.DeviceNo);
            }

            if (!AppData.Instance.IsActivate)
            {
                LoadingMessageText = "正在检查设备激活信息...";
                DeviceActiveResult deviceActiveResult= await cloudSyncService.GetActiveResultAsync(appSetting.DeviceInfo.DeviceNo);
                if (!deviceActiveResult.Result)
                {
                    MessageBoxService.Instance.Show("设备未激活，请联系供应商进行激活！", "设备未激活", MessageBoxButton.OK);
                    return false;
                }
                else
                {
                    AppData.Instance.DeviceActiveResult= deviceActiveResult;
                    AppData.Instance.IsActivate = true;
                }
            }
            LoadingMessageText = "正在检查硬件设备连接...";
            return true;
        }


        public DelegateCommand CSFCommand => new DelegateCommand(async () => await CSF());
        private async Task CSF()
        {
            if (!await CheckDeviceStatus())
            {
                return;
            }

            if (!AppData.Instance.IsLogin)
            {
                MessageBoxService.Instance.Show("请先登录！");
                return;
            }
            
            

            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_TrainFrame, new NavigationParameters() { { "type", MenuType.CSF } });
        }
        public DelegateCommand DEACommand => new DelegateCommand(async () => await DEA());
        private async Task DEA()
        {
            if (!await CheckDeviceStatus())
            {
                return;
            }
            if (!AppData.Instance.IsLogin)
            {
                MessageBoxService.Instance.Show("请先登录！");
                return;
            }
            
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_TrainFrame, new NavigationParameters() { { "type", MenuType.DEA } });
        }

        public DelegateCommand ConfigCommand => new DelegateCommand(Config);
        private void Config()
        {
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_TrainFrame, new NavigationParameters() { { "type", MenuType.Setting } });
        }

        public DelegateCommand ScanCommand => new DelegateCommand(async () => await Scan());

        private async Task Scan()
        {
            if (!AppData.Instance.IsConnectCloud)
            {
                MessageBoxService.Instance.Show("请先连接服务器！");
                return;
            }
            if (!AppData.Instance.IsRegister)
            {
                MessageBoxService.Instance.Show("请先注册设备！");
                return;
            }
            if (!AppData.Instance.IsActivate)
            {
                MessageBoxService.Instance.Show("请先激活设备！");
                return;
            }

            dialogService.ShowDialog(AppConst.Main_Dialog_Scan, new DialogParameters(), async result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    string checkid = result.Parameters.GetValue<string>("scanResult");

                    try
                    {
                        IsLoading= true;
                        LoadingMessageText = "正在查询检查方案信息...";
                        await GetSolutionInfo(new CloudSolutionDataItem() { Guid = checkid, Type = 0 });
                    }
                    finally
                    {
                        IsLoading = false;
                    }

                    
                }
            });
        }

        //根据解决方案编码获取解决方案信息
        private async Task GetSolutionInfo(CloudSolutionDataItem item)
        {
            AppData.Instance.PrescribeInfo = null;
            PrescribeInfo prescribeInfo = await cloudSyncService.GetPrescribeInfoAsync(item, AppData.Instance.DeviceNo, AppData.Instance.DeviceActiveResult.DataSecretKey);
            if (prescribeInfo != null)
            {
                AppData.Instance.PrescribeInfo = prescribeInfo;
                logger.LogInformation("获取到处方信息，开始执行");
                LoadingMessageText = "正在保存患者信息";
                await SavePatientInfo(prescribeInfo.Patient);
                LoadingMessageText = "正在保存医护信息";
                await SaveMangerInfo(prescribeInfo.Doctor);
                trainingAndCheckService.Initialize(prescribeInfo.ItemList);
                speechService.Speak("检查开始");
                trainingAndCheckService.StartTraining();
            }
            IsLoading = false;
        }

        private ItemInfoDto BuildStartItem(ItemOrder order)
        {
            ItemInfoDto item = new ItemInfoDto()
            {
                ItemId=order.ItemGuid,
                ParientIdCard = AppData.Instance.CurrentPatient.IdCard,
                PatientId = AppData.Instance.CurrentPatient.PatientIdNumber,
                RunParam = order.ItemParam.ToJson(),
                PatientName=AppData.Instance.CurrentPatient.PatientName,
                StartTime = DateTime.Now,
                PrescribeId= AppData.Instance.PrescribeInfo.PrescribeId,
                ItemType=0,
            };
            if (order.ItemGuid == AppConst.CheckItemCode_Contrast)//暗环境适应
            {
                item.Eye = Eye.OU.GetDescription();
                item.ItemName = "暗环境适应";
            }
            else if (order.ItemGuid == AppConst.CheckItemCode_Sensitivity)//对比敏感度
            {
                EyeTestParam eyeTestParam = (EyeTestParam)order.ItemParam;
                item.Eye = GetEyeText(eyeTestParam.EyeType);
                item.ItemName = "对比敏感度";
              

            }
            return item;
        }

        private string GetEyeText(int eyestype)
        {
            string[] arr = ["", "左眼", "右眼", "双眼"];
            if (eyestype > 0 && eyestype < 4)
            {
                return arr[eyestype];
            }
            else
            {
                return arr[3];
            }

        }


        private ItemStopDto<List<CheckResultDto>> BuildEndItem(ItemOrder order)
        {
            CheckResultDto checkResultDto = new CheckResultDto()
            {
                EndTime = order.EndTime,
                StartTime = order.StartTime,
                DoctorId = AppData.Instance.PrescribeInfo.Doctor.Id,
                CheckResult = order.Result,
                ItemID = order.ItemGuid,
                ItemName = order.ItemName,
            };
            if (order.ItemGuid == AppConst.CheckItemCode_Contrast)//暗环境适应
            {
                checkResultDto.EyeName = Eye.OU.GetDescription();
                checkResultDto.ItemName = "暗环境适应";
            }
            else if (order.ItemGuid == AppConst.CheckItemCode_Sensitivity)//对比敏感度
            {
                EyeTestParam eyeTestParam = (EyeTestParam)order.ItemParam;
                checkResultDto.EyeName = GetEyeText(eyeTestParam.EyeType);
                checkResultDto.ItemName = "对比敏感度";
            }
            ItemStopDto<List<CheckResultDto>> item = new ItemStopDto<List<CheckResultDto>>()
            {
                Data=new List<CheckResultDto>()
                {
                    checkResultDto
                },
                ItemID = order.ItemGuid,
                EndTime=order.EndTime,
                Eye=checkResultDto.EyeName,
                IdCard=AppData.Instance.CurrentPatient.IdCard,
                PrescribeId=AppData.Instance.PrescribeInfo.PrescribeId,
                PrescribeType=0,
                State=true,
                PatientId=AppData.Instance.CurrentPatient.PatientIdNumber,
            };
            return item;
        }

        private async void TrainingAndCheckService_TrainingItemStarted(object? sender, TrainingItemEventArgs e)
        {
            logger.LogInformation($"开始检查,{e.Item.ItemGuid},参数：{e.Item.ItemParam}");
            await speechService.SpeakAsync($"开始执行{e.Item.ItemName}检查");
            
            await cloudSyncService.ItemStart(new DeviceData()
            {
                DeviceNum=AppData.Instance.DeviceNo,
                Action=WebAction.ItemStart,

            }, BuildStartItem(e.Item)
            , AppData.Instance.DeviceActiveResult.DataSecretKey.DesEncrypt());
            #region 跳转到相应界面
            NavigationParameters navigationParameters = new NavigationParameters();
            navigationParameters.Add(nameof(TrainEnterMode), TrainEnterMode.FromList);
            if (e.Item.ItemGuid == AppConst.CheckItemCode_Contrast)
            {
                CheckTimesParam checkTimesParam = e.Item.ItemParamJson.ToObject<CheckTimesParam>();
                ContrastConfigParam contrastConfigParam = new ContrastConfigParam()
                {
                    PD = checkTimesParam.Pupillary,
                    CheckDuration = (DCKTime)checkTimesParam.CheckTimes
                };
               
                #region 初始化硬件
                await motor.Initialize();
                await motor.SetLeftDisk(3);
                await motor.SetRightDisk(3);
                await motor.SetSlideBlock((short)contrastConfigParam.PD);
                #endregion
                #region 硬件准备好，开始执行检查
                await Utils.WaitForConditionAsync(motor.IsAllStop, () =>
                {
                    speechService.SpeakAsync("开始训练,请选择能看清最大的视标编号");
                    NavigationParameters paramer=new NavigationParameters();
                    paramer.Add(nameof(ContrastConfigParam), contrastConfigParam);
                    paramer.Add(nameof(TrainEnterMode), TrainEnterMode.FromList);
                    regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_ContrastTraining, paramer);
                }, AppConst.WaitHardwareMotionTimeout);

                #endregion
            }
            else if(e.Item.ItemGuid== AppConst.CheckItemCode_Sensitivity)
            {
                EyeTestParam checkTimesParam = e.Item.ItemParamJson.ToObject<EyeTestParam>();
                SensitivityConfigParam contrastConfigParam = new SensitivityConfigParam()
                {
                    PD = checkTimesParam.Pupillary,
                    Eyes= (Eye)checkTimesParam.EyeType,
                    DayNight= (DayOrNight) checkTimesParam.TimeSlot,
                    IsLightOn= (LightStatus)checkTimesParam.DazzleLight,
                    CheckDistance=(CheckDistance)checkTimesParam.Distance

                };
                contrastConfigParam.CheckDuration = contrastConfigParam.Eyes == Eye.OU ? 60 : 30;
                #region 初始化硬件
                await motor.Initialize();
                await motor.SetLeftDisk(3);
                await motor.SetRightDisk(3);
                await motor.SetSlideBlock((short)contrastConfigParam.PD);
                #endregion
                #region 硬件准备好，开始执行检查
                await Utils.WaitForConditionAsync(motor.IsAllStop, () =>
                {
                    speechService.SpeakAsync("开始训练,请选择能看清最大的视标编号");
                    NavigationParameters paramer = new NavigationParameters();
                    paramer.Add(nameof(SensitivityConfigParam), contrastConfigParam);
                    paramer.Add(nameof(TrainEnterMode), TrainEnterMode.FromList);
                    regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_ContrastTraining, paramer);
                }, AppConst.WaitHardwareMotionTimeout);

                #endregion
            }

            #endregion

        }

        private void TrainingAndCheckService_TrainingItemCompleted(object? sender, TrainingItemEventArgs e)
        {
            logger.LogInformation($"结束检查,{e.Item.ItemGuid},参数：{e.Item.ItemParam}");
            cloudSyncService.ItemEnd(new DeviceData()
            {
                Action = WebAction.ItemEnd,
                DeviceNum = AppData.Instance.DeviceNo,
                
            },BuildEndItem(e.Item), AppData.Instance.DeviceActiveResult.DataSecretKey.DesEncrypt());

        }
        /// <summary>
        /// 训练结束
        /// 全部结束之后，退出登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrainingAndCheckService_TrainingFinished(object? sender, EventArgs e)
        {
            
        }

        private async Task SavePatientInfo(PatientInfoModel savePatientInfo)
        {
            int count = await patientRepository.CountAsync(n => n.PatientIdNumber == savePatientInfo.PatientIdNumber);
            if (count <= 0)
            {
                //执行保存操作
                await patientRepository.Add(new Patient()
                {
                    PatientIdNumber = savePatientInfo.PatientIdNumber,
                    IsDeleted = false,
                    Address = savePatientInfo.Address,
                    Age = savePatientInfo.Age,
                    Sex = savePatientInfo.Sex,
                    Grade = savePatientInfo.Grade,
                    IdCard = savePatientInfo.IdCard,
                    Class = savePatientInfo.Clas,
                    LoginName = savePatientInfo.PatientName,
                    CreateTime = DateTime.Now,
                    Phone = savePatientInfo.Phone,
                    School = savePatientInfo.School,
                    PatientName = savePatientInfo.PatientName,
                });
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="saveUserModel"></param>
        /// <returns></returns>
        private async Task SaveMangerInfo(UserModel saveUserModel)
        {
            int count = await mangerRepository.CountAsync(n => n.DoctorID == saveUserModel.DoctorId);
            if (count <= 0)
            {
                await mangerRepository.Add(new Manger()
                {

                    DoctorID = saveUserModel.DoctorId,
                    DoctorName = saveUserModel.DoctorName,
                    IsDeleted = false,
                    Gender = saveUserModel.Sex.ToString(),
                    Email = saveUserModel.Email,
                    Phone = saveUserModel.Phone,
                    Password = saveUserModel.Pwd,
                    Name = saveUserModel.Name,
                });

            }
        }



        public DelegateCommand LoginCommand => new DelegateCommand(Login);



        private void Login()
        {
            if (!AppData.Instance.IsConnectCloud)
            {
                MessageBoxService.Instance.Show("请先连接服务器！");
                return;
            }
            if (!AppData.Instance.IsRegister)
            {
                MessageBoxService.Instance.Show("请先注册设备！");
                return;
            }
            if (!AppData.Instance.IsActivate)
            {
                MessageBoxService.Instance.Show("请先激活设备！");
                return;
            }
            //登录
            dialogService.ShowDialog(AppConst.Main_Page_Login, new DialogParameters(), result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    AppData.Instance.IsLogin = true;
                    UserName = AppData.Instance.CurrentPatient.PatientName;
                    UserVisibility = Visibility.Visible;
                    
                }

            });

        }

        public DelegateCommand RegisterCommand=> new DelegateCommand(Register);

        private void Register()
        {
            if (!AppData.Instance.IsConnectCloud)
            {
                MessageBoxService.Instance.Show("请先连接服务器！");
                return;
            }
            if (!AppData.Instance.IsRegister)
            {
                MessageBoxService.Instance.Show("请先注册设备！");
                return;
            }
            if (!AppData.Instance.IsActivate)
            {
                MessageBoxService.Instance.Show("请先激活设备！");
                return;
            }
            dialogService.ShowDialog(AppConst.Main_Dialog_Register, new DialogParameters(), result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    AppData.Instance.IsLogin = true;
                    UserName = AppData.Instance.CurrentPatient.PatientName;
                    UserVisibility = Visibility.Visible;
                }
            });
        }


        public DelegateCommand ShutDownCommand => new DelegateCommand(ShutDown);

        private void ShutDown()
        {
            //关机
            if(MessageBoxService.Instance.ShowConfirm("确定要关闭系统吗？", "关闭系统")== MessageBoxResult.Yes)
            {
                Utils.ShutDown();
            }
        }

        public DelegateCommand RestartCommand=>new DelegateCommand(Restart);

        private void Restart()
        {
            //关机
            if (MessageBoxService.Instance.ShowConfirm("确定要重启系统吗？", "重启系统") == MessageBoxResult.Yes)
            {
                Utils.ShutDown();
            }
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive; set
            {
                _isActive = value;
                Console.WriteLine("IsActive changed to " + value);

            }
        }

        /// <summary>
        /// 直接跳转到检查页面
        /// </summary>
        private void JumpToCheck()
        {
            ContrastConfigParam contrastConfigParam = new ContrastConfigParam();
            contrastConfigParam.PD = 62;
            contrastConfigParam.CheckDuration = DCKTime.T05;
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_ContrastTraining, new NavigationParameters() { { nameof(ContrastConfigParam), contrastConfigParam } });


            //speechService.SpeakAsync("开始训练,请选择能看清最大的视标编号");
            SensitivityConfigParam sensitivityConfigParam= new SensitivityConfigParam();

            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_SensitivityTraining
                , new NavigationParameters() { { nameof(SensitivityConfigParam), sensitivityConfigParam } });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            // 获取单个参数
            if (navigationContext.Parameters.ContainsKey(AppConst.FinishItemKey))
            {
                MenuType MenuType = navigationContext.Parameters.GetValue<MenuType>(AppConst.FinishItemKey);
                trainingAndCheckService.CompleteCurrentItem();
            }
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
