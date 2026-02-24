using FSK.Sensitivity.Main.Views.Dialogs;
using Prism.Dialogs;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Main.Controls
{
    /// <summary>
    /// 消息框服务
    /// </summary>
    public class MessageBoxService
    {
        private static MessageBoxService _instance;
        public static MessageBoxService Instance => _instance ??= new MessageBoxService();
        private MessageBoxService() { }

        #region 原有方法（保持不变）

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

        #endregion

        #region 新增倒计时方法

        /// <summary>
        /// 显示带倒计时的消息框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="countDownSeconds">倒计时秒数</param>
        /// <param name="title">标题</param>
        /// <param name="buttons">按钮类型</param>
        /// <param name="messageType">消息类型</param>
        /// <param name="autoClickButton">倒计时完成时自动点击的按钮</param>
        /// <returns>用户选择的结果</returns>
        public MessageBoxResult ShowWithCountDown(
            string message,
            int countDownSeconds,
            string title = "提示",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageType messageType = MessageType.Info,
            MessageBoxResult autoClickButton = MessageBoxResult.OK)
        {
            if (countDownSeconds <= 0)
            {
                return Show(message, title, buttons, messageType);
            }

            var window = new MessageWindows
            {
                Message = message,
                Title = title,
                MessageType = messageType,
                CountDownSeconds = countDownSeconds,
                CountDownAction = autoClickButton
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
        /// 异步显示带倒计时的消息框
        /// </summary>
        public async Task<MessageBoxResult> ShowWithCountDownAsync(
            string message,
            int countDownSeconds,
            string title = "提示",
            MessageBoxButton buttons = MessageBoxButton.OK,
            MessageType messageType = MessageType.Info,
            MessageBoxResult autoClickButton = MessageBoxResult.OK)
        {
            return await Task.Run(() => ShowWithCountDown(message, countDownSeconds, title, buttons, messageType, autoClickButton));
        }

        /// <summary>
        /// 显示带倒计时的信息消息框
        /// </summary>
        public MessageBoxResult ShowInfoWithCountDown(string message, int countDownSeconds, string title = "信息")
        {
            return ShowWithCountDown(message, countDownSeconds, title, MessageBoxButton.OK, MessageType.Info, MessageBoxResult.OK);
        }

        /// <summary>
        /// 显示带倒计时的警告消息框
        /// </summary>
        public MessageBoxResult ShowWarningWithCountDown(string message, int countDownSeconds, string title = "警告")
        {
            return ShowWithCountDown(message, countDownSeconds, title, MessageBoxButton.OK, MessageType.Warning, MessageBoxResult.OK);
        }

        /// <summary>
        /// 显示带倒计时的错误消息框
        /// </summary>
        public MessageBoxResult ShowErrorWithCountDown(string message, int countDownSeconds, string title = "错误")
        {
            return ShowWithCountDown(message, countDownSeconds, title, MessageBoxButton.OK, MessageType.Error, MessageBoxResult.OK);
        }

        /// <summary>
        /// 显示带倒计时的确认消息框
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="countDownSeconds">倒计时秒数</param>
        /// <param name="title">标题</param>
        /// <param name="autoClickButton">倒计时完成时自动点击的按钮（Yes或No）</param>
        /// <returns>用户选择的结果</returns>
        public MessageBoxResult ShowConfirmWithCountDown(
            string message,
            int countDownSeconds,
            string title = "确认",
            MessageBoxResult autoClickButton = MessageBoxResult.Yes)
        {
            return ShowWithCountDown(message, countDownSeconds, title, MessageBoxButton.YesNo, MessageType.Question, autoClickButton);
        }

        #endregion

        #region 私有辅助方法

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

        #endregion

        #region 打开结束窗体

        public MessageBoxResult ShowFinishWindow()
        {
            
            
            return  MessageBoxResult.Cancel;
        }

        #endregion
    }
}
