using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    public interface ILight
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        bool IsAvailable { get; }
        /// <summary>
        /// 打开
        /// </summary>
        void TurnOn();
        /// <summary>
        /// 关闭
        /// </summary>
        void TurnOff();

        ///弱光
        /// <summary>
        /// 弱光
        /// </summary>
        void WeakLight();
    }
}
