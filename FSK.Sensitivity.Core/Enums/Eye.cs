using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    public enum Eye
    {
        /// <summary>
        /// 左眼
        /// </summary>
        [Description("左眼")]
        OS = 1,
        /// <summary>
        /// 右眼
        /// </summary>
        [Description("右眼")]
        OD,
        /// <summary>
        /// 双眼
        /// </summary>
        [Description("双眼")]
        OU,
    }
}
