using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.EventBus
{
    /// <summary>
    /// 主界面窗体切换事件
    /// </summary>
    public class MainContentChangeEvent : PubSubEvent<MainContentChangeOptions>
    {
    }
}
