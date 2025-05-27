using FSK.Sensitivity.Core.Const;
using HandyControl.Controls;
using Prism.Dialogs;
using Prism.Navigation.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
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
            set { SetProperty(ref _isLogin, value); }
        }

        public DelegateCommand CSFCommand => new DelegateCommand(CSF);
        private void CSF()
        {
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_TrainFrame, new NavigationParameters() { { "type", "CSF" } });
            regionManager.RequestNavigate(AppConst.SignRegion, AppConst.Sign_Page_Init);
        }
        public DelegateCommand DEACommand => new DelegateCommand(DEA);
        private void DEA()
        {
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
                    
                }
                else
                {
                    
                }
            });
        }


        public DelegateCommand LoginCommand => new DelegateCommand(Login);



        private void Login()
        {
            if (IsLogin)
            {
                //退出登录
                IsLogin = false;

            }
            else {
                //登录
                dialogService.ShowDialog(AppConst.Main_Page_Login, new DialogParameters(), result =>
                {
                    if (result.Result == ButtonResult.OK)
                    {
                        IsLogin = true;
                    }

                });
            }
        }

        public DelegateCommand RegisterCommand=> new DelegateCommand(Register);

        private void Register()
        {
            dialogService.ShowDialog(AppConst.Main_Dialog_Register, new DialogParameters(), result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    MessageBox.Show("注册成功");
                }
            });
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
