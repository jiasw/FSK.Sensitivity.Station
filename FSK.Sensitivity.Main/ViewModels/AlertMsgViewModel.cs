using FSK.Sensitivity.Core.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class AlertMsgViewModel : BaseViewModel, IDialogAware
    {
        private string title = "提示";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }
        private string _msg="消息内容";
        public string Msg
        {
            get { return _msg; }
            set { SetProperty(ref _msg, value); }
        }

        public DialogCloseListener RequestClose {get; }

        public DelegateCommand CloseCommand=>new DelegateCommand(Close);

        private void Close()
        {
            RequestClose.Invoke();
        }


        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
             
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters.ContainsKey(AppConst.Main_Dialog_AlertMsg))
            {
                Msg = parameters.GetValue<string>(AppConst.Main_Dialog_AlertMsg);
            }
        }
    }
}
