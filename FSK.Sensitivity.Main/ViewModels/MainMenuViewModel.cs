using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
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
            logger.LogInformation("开始注册");

            DeviceActiveResult result = await cloudSyncService.GetActiveResultAsync("FSK20260129003");
            //scanner.Stop();
            //DeviceRegistResult RE = await cloudSyncService.RegisterDevice<DeviceRegistResult>();
            if (!secureRegistrationService.IsRegistered())
            {
                
            }
            CheckHardware();

        }
        public DelegateCommand ExitCommand=> new DelegateCommand(ExitLogin);

        private void ExitLogin()
        {
            AppData.Instance.CurrentPatient = null;
            AppData.Instance.IsLogin = false;
            UserVisibility = Visibility.Collapsed;
            LoginVisibility = Visibility.Visible;
        }

        /// <summary>
        /// 设备注册
        /// </summary>
        private void RegisterHardware()
        {
            if (string.IsNullOrWhiteSpace(appSetting.DeviceInfo.DeviceNo))
            {
                MessageBoxResult messageBoxResult = MessageBoxService.Instance.ShowConfirm("是否初始化系统？");
                if (messageBoxResult == MessageBoxResult.Yes)
                {
                    
                }
                else
                {
                    AppData.Instance.IsRegister = false;
                }
            }
            

        }

        private void CheckHardware()
        {
            RegisterHardware();
            IsLoading = true;
            if (!modbusService.IsConnected)
            {
                modbusService.Initialize(appSetting.ModbusPortName);
                 modbusService.Connect();
            }
           
            if (!modbusService.IsConnected)
            {
                MessageBoxService.Instance.Show("硬件设备未连接，请连接后重试！", "设备未连接", MessageBoxButton.OK);
            }


            IsLoading = false;
        }


        public DelegateCommand CSFCommand => new DelegateCommand(CSF);
        private void CSF()
        {
            if (!AppData.Instance.IsLogin)
            {
                MessageBoxService.Instance.Show("请先登录！");
                return;
            }
            if (!AppData.Instance.IsRegister)
            {
                MessageBoxService.Instance.Show("请先注册设备！");
                return;
            }
            if (!modbusService.IsConnected)
            {
                MessageBoxService.Instance.Show("硬件设备未连接，请连接后重试！");
                return;
            }

            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_TrainFrame, new NavigationParameters() { { "type", MenuType.CSF } });
        }
        public DelegateCommand DEACommand => new DelegateCommand(DEA);
        private void DEA()
        {
            if (!AppData.Instance.IsLogin)
            {
                MessageBoxService.Instance.Show("请先登录！");
                return;
            }
            if (!AppData.Instance.IsRegister)
            {
                MessageBoxService.Instance.Show("请先注册设备！");
                return;
            }
            if (!modbusService.IsConnected)
            {
                MessageBoxService.Instance.Show("硬件设备未连接，请连接后重试！");
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

        
    }
}
