using FSK.Sensitivity.Core.HardWare.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    /// <summary>
    /// 手柄
    /// </summary>
    public interface IJoystick
    {
        public event EventHandler<JoystickEventArgs> Pressed;

        /// <summary>
        /// 开始监听
        /// </summary>
        void StartMonitoring();


        /// <summary>
        /// 停止监听
        /// </summary>
        void StopMonitoring();


    }
}
