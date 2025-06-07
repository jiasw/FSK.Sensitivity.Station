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
        None,
        VA06,
        VA10,
        VA20,
        VA40,
        VA60,
        VA80
    }

    
}
