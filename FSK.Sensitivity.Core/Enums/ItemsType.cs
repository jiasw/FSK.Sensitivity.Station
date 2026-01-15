using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    /// <summary>
    /// 选择项目类型
    /// </summary>
    public enum ItemsType
    {
        [Description("眼别")]
        Eyes=1,
        [Description("日夜")]
        DayTypes = 2,
        [Description("距离")]
        Distance = 3,
        [Description("瞳距")]
        PD = 4,
        [Description("暗环境时长")]
        DarkTime = 5
    }
}
