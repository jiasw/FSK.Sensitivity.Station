using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Enums
{
    public enum UserType
    {
        [Description("管理员")]
        Admin=0,
        [Description("操作员")]
        User
    }

    public enum Gender
    {
        [Description("女 ")]
        Female=0,
        [Description("男 ")]
        Male = 1
    }

    /// <summary>
    /// 检查结果
    /// </summary>
    public enum CheckResultEnums
    {

        [Description("正常")]
        Normal=0,
        [Description("异常")]
        Abnormal

    }

    public enum MenuType
    {
        [Description("对比敏感度")]
        CSF=0,
        [Description("暗环境适应")]
        DEA,
        [Description("设置")]
        Setting

    }
}
