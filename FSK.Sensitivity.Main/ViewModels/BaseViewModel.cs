using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class BaseViewModel : BindableBase
    {
        private bool Loading = false;
        public bool IsLoading
        {
            get { return Loading; }
            set { SetProperty(ref Loading, value); }
        }
        private string LoadingMessage = "正在加载中...";
        public string LoadingMessageText
        {
            get { return LoadingMessage; }
            set { SetProperty(ref LoadingMessage, value); } 
        }
    }
}
