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
        public DateTime Timestamp { get; set; }
        public string Command { get; set; }
    }
}
