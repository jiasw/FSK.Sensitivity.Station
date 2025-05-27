using FSK.Sensitivity.Core.Const;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class LoginViewModel : BaseViewModel, IDialogAware
    {
        private string title = "登录";
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        private string username;
        public string Username
        {
            get => username;
            set => SetProperty(ref username, value);
        }   
        private string password;
        private readonly IDialogService dialogService;

        public LoginViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }

        public string Password
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        public DialogCloseListener RequestClose { get; }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
        public virtual void RaiseRequestClose(IDialogResult dialogResult)
        {
            RequestClose.Invoke(dialogResult);
        }

        public DelegateCommand LoginCommand => new DelegateCommand(Login);

        private void Login()
        {
            //TODO: 登录逻辑    
            RaiseRequestClose(new DialogResult(ButtonResult.OK));
        }

        public DelegateCommand CancelCommand => new DelegateCommand(Cancel);

        private void Cancel()
        {
            RaiseRequestClose(new DialogResult(ButtonResult.Cancel));
        }

        public DelegateCommand RegiserCommand => new DelegateCommand(Regiser);

        private void Regiser()
        {
            //TODO: 注册逻辑
            dialogService.ShowDialog(AppConst.Main_Dialog_Register, new DialogParameters(), result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                   
                }

            });
        }
    }
}
