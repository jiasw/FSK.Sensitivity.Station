using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Main.Controls;
using HandyControl.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FSK.Sensitivity.Main.Views
{
    /// <summary>
    /// NetWork.xaml 的交互逻辑
    /// </summary>
    public partial class NetWork : UserControl
    {
        private readonly Iwifi iwifi;

        public NetWork(Iwifi iwifi)
        {
            InitializeComponent();
            this.iwifi = iwifi;
        }

        private void connectCompleted(bool result)
        {
            if (result)
            {
                Dispatcher.Invoke(() => MessageBoxService.Instance.ShowInfo("连接成功"));
            }
            else
            {
                Dispatcher.Invoke(() => MessageBoxService.Instance.ShowWarning("连接失败，请检查密码是否正确"));
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            // 1. 在 UI 线程预先取值
            string ssid = cbxSelectWIFI.SelectedValue?.ToString();
            string password = txtMM.Text;

            if (string.IsNullOrEmpty(ssid)) return;
            // 这里可以添加：IsBusy = true; (禁用按钮)

             iwifi.Connect(ssid, password, connectCompleted);
            Growl.Info("开始连接,请稍候...");
           
        }
    }
}
