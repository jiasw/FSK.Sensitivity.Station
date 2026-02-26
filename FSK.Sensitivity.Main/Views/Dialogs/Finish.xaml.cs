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
using WpfScreenHelper;

namespace FSK.Sensitivity.Main.Views.Dialogs
{
    /// <summary>
    /// Finish.xaml 的交互逻辑
    /// </summary>
    public partial class Finish : UserControl
    {
        public Finish()
        {
            InitializeComponent();
        }

        // 事件
        public event EventHandler<MessageBoxResult> DialogResultEvent;
        // 按钮点击事件
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.OK);
            
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.Cancel);
            
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.Yes);
            
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.No);
            
        }
        // 窗口拖拽
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                
            }
        }
        // 关闭按钮
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.None);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                SetWindowToMainScreen(window);
            }
        }

        private void SetWindowToMainScreen(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;

            var primaryScreen = Screen.PrimaryScreen;
            window.Left = primaryScreen.WorkingArea.Left +
                         (primaryScreen.WorkingArea.Width - window.Width) / 2;
            window.Top = primaryScreen.WorkingArea.Top +
                        (primaryScreen.WorkingArea.Height - window.Height) / 2;

            window.Topmost = true;
            window.Topmost = false;
        }
    }
}
