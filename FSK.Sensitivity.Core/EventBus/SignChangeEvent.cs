using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.EventBus
{
    /// <summary>
    /// 视标切换事件
    /// </summary>
    public class SignChangeEvent : PubSubEvent<string>
    {
    }
}
