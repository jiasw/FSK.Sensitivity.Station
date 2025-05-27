using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class ScanViewModel : BaseViewModel, IDialogAware
    {
        private string title="扫描登录";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        public DialogCloseListener RequestClose  {get;}

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

        public DelegateCommand CloseCommand => new DelegateCommand(Close);

        private void Close()
        {
            RequestClose.Invoke();
        }
    }
}
