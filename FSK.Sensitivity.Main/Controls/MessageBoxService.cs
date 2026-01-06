using FSK.Sensitivity.Main.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Main.Controls
{
    /// <summary>
    /// 消息弹窗服务
    /// </summary>
    public class MessageBoxService
    {
        private static MessageBoxService _instance;
        public static MessageBoxService Instance => _instance ??= new MessageBoxService();
        private MessageBoxService() { }
        /// <summary>
        /// 显示消息框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="title">标题</param>
        /// <param name="buttons">按钮类型</param>
        /// <param name="messageType">消息类型</param>
        /// <returns>用户选择的结果</returns>
        public MessageBoxResult Show(string message, string title = "提示", MessageBoxButton buttons = MessageBoxButton.OK, MessageType messageType = MessageType.Info)
        {
            var window = new MessageWindows
            {
                Message = message,
                Title = title,
                MessageType = messageType
            };
            SetButtonsVisibility(window, buttons);
            var owner = Application.Current?.MainWindow;
            if (owner != null && owner.IsVisible)
            {
                window.Owner = owner;
            }
            var result = window.ShowDialog();
            return result.HasValue && result.Value ? GetPositiveResult(buttons) : GetNegativeResult(buttons);
        }
        /// <summary>
        /// 异步显示消息框
        /// </summary>
        public async Task<MessageBoxResult> ShowAsync(string message, string title = "提示", MessageBoxButton buttons = MessageBoxButton.OK, MessageType messageType = MessageType.Info)
        {
            return await Task.Run(() => Show(message, title, buttons, messageType));
        }
        /// <summary>
        /// 显示信息消息框
        /// </summary>
        public MessageBoxResult ShowInfo(string message, string title = "信息")
        {
            return Show(message, title, MessageBoxButton.OK, MessageType.Info);
        }
        /// <summary>
        /// 显示警告消息框
        /// </summary>
        public MessageBoxResult ShowWarning(string message, string title = "警告")
        {
            return Show(message, title, MessageBoxButton.OK, MessageType.Warning);
        }
        /// <summary>
        /// 显示错误消息框
        /// </summary>
        public MessageBoxResult ShowError(string message, string title = "错误")
        {
            return Show(message, title, MessageBoxButton.OK, MessageType.Error);
        }
        /// <summary>
        /// 显示确认消息框
        /// </summary>
        public MessageBoxResult ShowConfirm(string message, string title = "确认")
        {
            return Show(message, title, MessageBoxButton.YesNo, MessageType.Question);
        }
        /// <summary>
        /// 根据按钮类型设置按钮可见性
        /// </summary>
        private void SetButtonsVisibility(MessageWindows window, MessageBoxButton buttons)
        {
            switch (buttons)
            {
                case MessageBoxButton.OK:
                    window.OkButton.Visibility = Visibility.Visible;
                    window.CancelButton.Visibility = Visibility.Collapsed;
                    window.YesButton.Visibility = Visibility.Collapsed;
                    window.NoButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.OKCancel:
                    window.OkButton.Visibility = Visibility.Visible;
                    window.CancelButton.Visibility = Visibility.Visible;
                    window.YesButton.Visibility = Visibility.Collapsed;
                    window.NoButton.Visibility = Visibility.Collapsed;
                    break;
                case MessageBoxButton.YesNo:
                    window.OkButton.Visibility = Visibility.Collapsed;
                    window.CancelButton.Visibility = Visibility.Collapsed;
                    window.YesButton.Visibility = Visibility.Visible;
                    window.NoButton.Visibility = Visibility.Visible;
                    break;
                case MessageBoxButton.YesNoCancel:
                    window.OkButton.Visibility = Visibility.Collapsed;
                    window.CancelButton.Visibility = Visibility.Visible;
                    window.YesButton.Visibility = Visibility.Visible;
                    window.NoButton.Visibility = Visibility.Visible;
                    break;
            }
        }
        /// <summary>
        /// 获取积极结果
        /// </summary>
        private MessageBoxResult GetPositiveResult(MessageBoxButton buttons)
        {
            return buttons switch
            {
                MessageBoxButton.OK => MessageBoxResult.OK,
                MessageBoxButton.YesNo => MessageBoxResult.Yes,
                MessageBoxButton.OKCancel => MessageBoxResult.OK,
                MessageBoxButton.YesNoCancel => MessageBoxResult.Yes,
                _ => MessageBoxResult.None
            };
        }
        /// <summary>
        /// 获取消极结果
        /// </summary>
        private MessageBoxResult GetNegativeResult(MessageBoxButton buttons)
        {
            return buttons switch
            {
                MessageBoxButton.OK => MessageBoxResult.OK,
                MessageBoxButton.YesNo => MessageBoxResult.No,
                MessageBoxButton.OKCancel => MessageBoxResult.Cancel,
                MessageBoxButton.YesNoCancel => MessageBoxResult.No,
                _ => MessageBoxResult.None
            };
        }
    }
   
}
