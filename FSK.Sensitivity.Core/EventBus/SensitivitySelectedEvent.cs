using FSK.Sensitivity.Core.HardWare.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.EventBus
{

    public class ActionArgs
    {
        public ActionArgs() { }

        /// <summary>
        /// 当前视标索引，默认为0
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// 手柄指令
        /// </summary>
        public JoystickEventArgs Action { get; set; }
    }

    /// <summary>
    /// 手柄事件
    /// </summary>
    
    public class JoystickEvent : PubSubEvent<ActionArgs> { 
        
    }
}
