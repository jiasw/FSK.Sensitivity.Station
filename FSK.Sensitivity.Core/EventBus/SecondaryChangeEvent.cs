using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.EventBus
{
    /// <summary>
    /// 副屏切换事件
    /// 用于切换对比度检查和暗环境检测两种情况
    /// </summary>
    public class SecondaryChangeEvent : PubSubEvent<SecondaryChangeOptions>
    {
    }
}
