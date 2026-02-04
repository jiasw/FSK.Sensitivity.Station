using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using FSK.Sensitivity.Main.Controls;
using HandyControl.Controls;
using Microsoft.Extensions.Logging;
using Prism.Dialogs;
using Prism.Navigation.Regions;
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
    public class MainMenuViewModel : BaseViewModel,IActiveAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly IModbusService modbusService;
        private readonly IConfigurationService configurationService;
        private readonly ISecureRegistrationService secureRegistrationService;
        private readonly ICloudSyncService cloudSyncService;
        private readonly ILogger<MainMenuViewModel> logger;
        private readonly AppSetting appSetting;
        private System.Timers.Timer checkNetWorkTimer;

        public MainMenuViewModel(IRegionManager regionManager, IDialogService dialogService
            , IModbusService modbusService, IConfigurationService configurationService
            , ISecureRegistrationService secureRegistrationService, ICloudSyncService cloudSyncService,ILogger<MainMenuViewModel> logger)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.modbusService = modbusService;
            this.configurationService = configurationService;
            this.secureRegistrationService = secureRegistrationService;
            this.cloudSyncService = cloudSyncService;
            this.logger = logger;
            this.appSetting = configurationService.LoadSetting();
            AppData.Instance.DialogService = dialogService;
            checkNetWorkTimer = new System.Timers.Timer(10000); // 设置定时器间隔为5秒
            checkNetWorkTimer.Elapsed += new ElapsedEventHandler(CheckNetWork);
            checkNetWorkTimer.Start();
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
                    }
                    AppData.Instance.IsRegister = true;
                }
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

        public DelegateCommand ScanCommand => new DelegateCommand(Scan);

        private void Scan()
        {
            dialogService.ShowDialog(AppConst.Main_Dialog_Scan, new DialogParameters(), result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    AppData.Instance.IsLogin = true;
                    UserName = AppData.Instance.CurrentPatient.PatientName;
                }
            });
        }


        public DelegateCommand LoginCommand => new DelegateCommand(Login);



        private void Login()
        {
            //dialogService.ShowDialog("HardWareTest");
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

        
    }
}
