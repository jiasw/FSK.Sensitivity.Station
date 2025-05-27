using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    /// <summary>
    /// 检查距离
    /// </summary>
    public enum CheckDistance
    {
        [Description("0.4")]
        Short =1,
        [Description("0.8")]
        Medium =2,
        [Description("1.5")]
        Long =3,
        [Description("5")]
        VeryLong =4

    }
}
