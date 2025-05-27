using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FSK.Sensitivity.Core.Model
{
    /// <summary>
    /// 摇杆指示按钮
    /// </summary>
    public class ArrowButtonModel : BindableBase
    {
        public string Name { get; set; }

        private bool _isButtonActive;
        public bool IsButtonActive
        {
            get { return _isButtonActive; }
            set { SetProperty(ref _isButtonActive, value); }
        }

        public DelegateCommand TriggerCommand => new DelegateCommand(Trigger);

        private void Trigger()
        {
            if (IsButtonActive)
            {
                return;
            }
            // 使用Task延迟确保动画完整执行
            Task.Run(async () =>
            {
                IsButtonActive = true;
                await Task.Delay(320); // 动画持续时间
                IsButtonActive = false;
            });
        }

    }
}
