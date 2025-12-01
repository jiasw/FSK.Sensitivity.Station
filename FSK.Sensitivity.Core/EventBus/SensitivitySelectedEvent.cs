using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.EventBus
{
    /// <summary>
    /// 敏感度检测时用户在副屏选择事件
    /// </summary>
    public class SensitivitySelectedEvent : PubSubEvent<int>
    {
    }

    /// <summary>
    /// 暗环境检测时用户在副屏选择事件
    /// </summary>
    public class ContrastSelectedEvent : PubSubEvent<int>
    {
        
    }
}
