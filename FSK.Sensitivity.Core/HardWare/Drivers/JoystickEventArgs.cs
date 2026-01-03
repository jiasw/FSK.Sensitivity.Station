using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Drivers
{
    //手柄事件参数
    public class JoystickEventArgs
    {
        public JoystickEventArgs(JoystickStatus command)
        {
            Timestamp = DateTime.Now;
            Command = command;
        }

        public DateTime Timestamp { get; set; }
        public JoystickStatus Command { get; set; }
    }
}
