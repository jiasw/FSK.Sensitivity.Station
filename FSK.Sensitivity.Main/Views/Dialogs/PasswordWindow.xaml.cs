using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
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
using System.Windows.Shapes;

namespace FSK.Sensitivity.Main.Views.Dialogs
{
    /// <summary>
    /// PasswordWindow.xaml 的交互逻辑
    /// </summary>
    public partial class PasswordWindow : System.Windows.Window
    {
        private readonly IConfigurationService configurationService;

        public PasswordWindow(IConfigurationService configurationService)
        {
            InitializeComponent();
            this.configurationService = configurationService;
        }



        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(PasswordBox.Text))
            {
                Growl.Warning("请输入密码！");
                return;
            }
            
            if (string.Compare(PasswordBox.Text, configurationService.LoadSetting().ComfirmPassword) !=0)
            {
                Growl.Warning("密码错误，请重新输入！");
                return;
            }

            this.DialogResult = true;
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
           
            this.DialogResult = false;
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }
        // 关闭按钮
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
