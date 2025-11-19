using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    /// <summary>
    /// 炫光灯状态
    /// </summary>
    public enum LightStatus
    {
        [Description("关闭")]
        Off =0,
        [Description("强哥")]
        Strong = 1,
        [Description("弱光")]
        Weak = 2
    }
}
