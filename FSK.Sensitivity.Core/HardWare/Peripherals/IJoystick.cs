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
        bool IsAvailable { get; }
        public event EventHandler<JoystickEventArgs> UpPressed;
        public event EventHandler<JoystickEventArgs> DownPressed;
        public event EventHandler<JoystickEventArgs> LeftPressed;
        public event EventHandler<JoystickEventArgs> RightPressed;
        public event EventHandler<JoystickEventArgs> TriggerPressed;
        public event EventHandler<JoystickEventArgs> ConfirmPressed;
    }
}
