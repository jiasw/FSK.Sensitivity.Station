using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Utility;
using HandyControl.Controls;
using Prism.Dialogs;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class MainMenuViewModel : BaseViewModel,IActiveAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        public MainMenuViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
        }

        private string _title = "主菜单";

        public event EventHandler IsActiveChanged;

        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }


        private bool _isLogin;
        public bool IsLogin
        {
            get { return _isLogin; }
            set
            {
                if (value)
                {
                    LoginVisibility = Visibility.Collapsed;
                    UserVisibility = Visibility.Visible;
                }
                else
                {
                    LoginVisibility = Visibility.Visible;
                    UserVisibility = Visibility.Collapsed;
                }

                SetProperty(ref _isLogin, value);
            }
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
            set { SetProperty(ref _userVisibility, value); }
        }

        private string _userName;
        public string UserName
        {
            get { return _userName; }
            set { SetProperty(ref _userName, value); }
        }

        public DelegateCommand ExitCommand=> new DelegateCommand(ExitLogin);

        private void ExitLogin()
        {
            AppData.Instance.CurrentPatient = null;
            IsLogin = false;
        }

        public DelegateCommand CSFCommand => new DelegateCommand(CSF);
        private void CSF()
        {
            if (!IsLogin)
            {
                HandyControl.Controls.MessageBox.Show("请先登录！");
                return;
            }
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_TrainFrame, new NavigationParameters() { { "type", "CSF" } });
        }
        public DelegateCommand DEACommand => new DelegateCommand(DEA);
        private void DEA()
        {
            if (!IsLogin)
            {
                HandyControl.Controls.MessageBox.Show("请先登录！");
                return;
            }
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_TrainFrame, new NavigationParameters() { { "type", "DEA" } });
        }

        public DelegateCommand ConfigCommand => new DelegateCommand(Config);
        private void Config()
        {
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_TrainFrame, new NavigationParameters() { { "type", "Setting" } });
        }

        public DelegateCommand ScanCommand => new DelegateCommand(Scan);

        private void Scan()
        {
            dialogService.ShowDialog(AppConst.Main_Dialog_Scan, new DialogParameters(), result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    IsLogin = true;
                    UserName = AppData.Instance.CurrentPatient.PatientName;
                }
            });
        }


        public DelegateCommand LoginCommand => new DelegateCommand(Login);



        private void Login()
        {

            //登录
            dialogService.ShowDialog(AppConst.Main_Page_Login, new DialogParameters(), result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    IsLogin = true;
                    UserName = AppData.Instance.CurrentPatient.PatientName;
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
                   IsLogin = true;
                    UserName = AppData.Instance.CurrentPatient.PatientName;
                }
            });
        }


        public DelegateCommand ShutDownCommand => new DelegateCommand(ShutDown);

        private void ShutDown()
        {
            //关机
            if(System.Windows.MessageBox.Show("确定要关闭系统吗？", "关闭系统", MessageBoxButton.OKCancel, MessageBoxImage.Question)== MessageBoxResult.OK)
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
