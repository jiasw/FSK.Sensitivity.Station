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
}
