using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.EventBus
{
    
    /// <summary>
    /// 敏感度视标切换事件
    /// </summary>
    public class SensitivitySignChangeEvent : PubSubEvent<SensitivityChangeSignOptions>
    {

    }
    /// <summary>
    /// 对比度视标切换事件
    /// </summary>
    public class ContrastSignChangeEvent : PubSubEvent<ContrastChangeSignOptions>
    {
    }
}
