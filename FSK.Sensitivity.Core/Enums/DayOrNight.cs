using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    public enum DayOrNight
    {
        [Description("日")]
        Day =1,
        [Description("夜")]
        Night
    }

    /// <summary>
    /// 环境模拟CSF检查
    /// </summary>
    public enum VAValue
    {
        [Description("空")]
        None,
        [Description("0.06")]
        VA06,
        [Description("0.1")]
        VA10,
        [Description("0.2")]
        VA20,
        [Description("0.4")]
        VA40,
        [Description("0.6")]
        VA60,
        [Description("0.8")]
        VA80
    }

    
}
