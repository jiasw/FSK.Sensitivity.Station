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
    /// MessageWindows.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindows : Window
    {
        public MessageWindows()
        {
            InitializeComponent();
            this.DataContext = this;

            // 设置窗口样式
            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            this.ResizeMode = ResizeMode.NoResize;
            this.WindowStyle = WindowStyle.None;
            this.AllowsTransparency = true;
            this.Background = Brushes.Transparent;
        }

        // 依赖属性
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageWindows), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MessageWindows), new PropertyMetadata("提示"));
        public static readonly DependencyProperty MessageTypeProperty =
            DependencyProperty.Register("MessageType", typeof(MessageType), typeof(MessageWindows), new PropertyMetadata(MessageType.Info, OnMessageTypeChanged));
        public string Message
        {
            get { return (string)GetValue(MessageProperty); }
            set { SetValue(MessageProperty, value); }
        }
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public MessageType MessageType
        {
            get { return (MessageType)GetValue(MessageTypeProperty); }
            set { SetValue(MessageTypeProperty, value); }
        }
        // 事件
        public event EventHandler<MessageBoxResult> DialogResultEvent;
        // 按钮点击事件
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.OK);
            this.DialogResult = true;
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.Cancel);
            this.DialogResult = false;
            this.Close();
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.Yes);
            this.DialogResult = true;
            this.Close();
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResultEvent?.Invoke(this, MessageBoxResult.No);
            this.DialogResult = false;
            this.Close();
        }
        // 窗口拖拽
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
        // 根据消息类型更新UI
        private static void OnMessageTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MessageWindows;
            if (window != null)
            {
                window.UpdateMessageTypeStyle();
            }
        }
        private void UpdateMessageTypeStyle()
        {
            // 这里可以根据消息类型设置不同的样式
            switch (MessageType)
            {
                case MessageType.Info:
                    // 设置信息样式
                    break;
                case MessageType.Warning:
                    // 设置警告样式
                    break;
                case MessageType.Error:
                    // 设置错误样式
                    break;
                case MessageType.Question:
                    // 设置问题样式
                    break;
            }
        }
    }
    // 消息类型枚举
    public enum MessageType
    {
        Info,
        Warning,
        Error,
        Question
    }

}
