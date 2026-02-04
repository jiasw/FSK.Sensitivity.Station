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
using System.Windows.Threading;

namespace FSK.Sensitivity.Main.Views.Dialogs
{
    /// <summary>
    /// MessageWindows.xaml 的交互逻辑
    /// </summary>
    public partial class MessageWindows : Window
    {
        private DispatcherTimer _countDownTimer;
        private int _remainingSeconds;
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
            // 初始化倒计时计时器
            _countDownTimer = new DispatcherTimer();
            _countDownTimer.Interval = TimeSpan.FromSeconds(1);
            _countDownTimer.Tick += CountDownTimer_Tick;
        }

        // 依赖属性
        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register("Message", typeof(string), typeof(MessageWindows), new PropertyMetadata(string.Empty));
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MessageWindows), new PropertyMetadata("提示"));
        public static readonly DependencyProperty MessageTypeProperty =
            DependencyProperty.Register("MessageType", typeof(MessageType), typeof(MessageWindows), new PropertyMetadata(MessageType.Info, OnMessageTypeChanged));
        /// <summary>
        /// 倒计时秒数（0表示不使用倒计时）
        /// </summary>
        public static readonly DependencyProperty CountDownSecondsProperty =
            DependencyProperty.Register("CountDownSeconds", typeof(int), typeof(MessageWindows),
                new PropertyMetadata(0, OnCountDownSecondsChanged));
        /// <summary>
        /// 倒计时完成时自动执行的操作
        /// </summary>
        public static readonly DependencyProperty CountDownActionProperty =
            DependencyProperty.Register("CountDownAction", typeof(MessageBoxResult), typeof(MessageWindows),
                new PropertyMetadata(MessageBoxResult.OK));
        /// <summary>
        /// 是否显示倒计时
        /// </summary>
        public static readonly DependencyProperty IsShowCountDownProperty =
            DependencyProperty.Register("IsShowCountDown", typeof(bool), typeof(MessageWindows),
                new PropertyMetadata(false));
        /// <summary>
        /// 倒计时文本
        /// </summary>
        public static readonly DependencyProperty CountDownTextProperty =
            DependencyProperty.Register("CountDownText", typeof(string), typeof(MessageWindows),
                new PropertyMetadata(""));
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

        public int CountDownSeconds
        {
            get { return (int)GetValue(CountDownSecondsProperty); }
            set { SetValue(CountDownSecondsProperty, value); }
        }
        public MessageBoxResult CountDownAction
        {
            get { return (MessageBoxResult)GetValue(CountDownActionProperty); }
            set { SetValue(CountDownActionProperty, value); }
        }
        public bool IsShowCountDown
        {
            get { return (bool)GetValue(IsShowCountDownProperty); }
            set { SetValue(IsShowCountDownProperty, value); }
        }
        public string CountDownText
        {
            get { return (string)GetValue(CountDownTextProperty); }
            set { SetValue(CountDownTextProperty, value); }
        }

        // 事件
        public event EventHandler<MessageBoxResult> DialogResultEvent;


        private static void OnCountDownSecondsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var window = d as MessageWindows;
            if (window != null)
            {
                window.StartCountDown();
            }
        }
        private void StartCountDown()
        {
            if (CountDownSeconds <= 0)
            {
                IsShowCountDown = false;
                _countDownTimer.Stop();
                return;
            }
            _remainingSeconds = CountDownSeconds;
            IsShowCountDown = true;
            UpdateCountDownText();
            _countDownTimer.Start();
        }
        private void CountDownTimer_Tick(object sender, EventArgs e)
        {
            _remainingSeconds--;
            UpdateCountDownText();
            if (_remainingSeconds <= 0)
            {
                _countDownTimer.Stop();
                ExecuteCountDownAction();
            }
        }

        private void UpdateCountDownText()
        {
            CountDownText = $"({_remainingSeconds}s)";
        }
        private void ExecuteCountDownAction()
        {
            switch (CountDownAction)
            {
                case MessageBoxResult.OK:
                    OkButton_Click(OkButton, null);
                    break;
                case MessageBoxResult.Cancel:
                    CancelButton_Click(CancelButton, null);
                    break;
                case MessageBoxResult.Yes:
                    YesButton_Click(YesButton, null);
                    break;
                case MessageBoxResult.No:
                    NoButton_Click(NoButton, null);
                    break;
            }
        }
        // 按钮点击事件
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _countDownTimer.Stop();
            DialogResultEvent?.Invoke(this, MessageBoxResult.OK);
            this.DialogResult = true;
            this.Close();
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            _countDownTimer.Stop();
            DialogResultEvent?.Invoke(this, MessageBoxResult.Cancel);
            this.DialogResult = false;
            this.Close();
        }
        private void YesButton_Click(object sender, RoutedEventArgs e)
        {
            _countDownTimer.Stop();
            DialogResultEvent?.Invoke(this, MessageBoxResult.Yes);
            this.DialogResult = true;
            this.Close();
        }
        private void NoButton_Click(object sender, RoutedEventArgs e)
        {
            _countDownTimer.Stop();
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
