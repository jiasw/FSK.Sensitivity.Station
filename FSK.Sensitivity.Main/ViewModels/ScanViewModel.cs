using FSK.Sensitivity.Core.HardWare.Peripherals;
using NetTaste;
using Serilog;
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

        public ScanViewModel(IScanner scanner)
        {
            Scanner = scanner;
            
        }

        private void Scanner_ScanCompleted(string result)
        {
            Log.Information($"扫描结果:{result}");
        }

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
            Scanner.Stop();
            Scanner.ScanCompleted -= Scanner_ScanCompleted;

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Scanner.Start();
            Scanner.ScanCompleted += Scanner_ScanCompleted;

        }

        public DelegateCommand CloseCommand => new DelegateCommand(Close);

        public IScanner Scanner { get; }

        private void Close()
        {
            RequestClose.Invoke();
        }


        private void ExecuteConfirm()
        {
            // 执行确认逻辑

            //var result = new DialogResult(ButtonResult.OK);
            // 或者带参数的返回
             var result = new DialogResult(ButtonResult.OK);
             result.Parameters.Add("scanResult", "112233");

            RequestClose.Invoke(result);
        }
    }
}
