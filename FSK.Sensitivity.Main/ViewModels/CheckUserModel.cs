using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    /// <summary>
    /// 检查人员信息
    /// </summary>
    public class CheckUserModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Age { get; set; }

        public string Gender { get; set; }

        public string PD { get; set; }

        public string Light { get; set; }

        public DayOrNight DayNight { get; set; }

        public string DayNigthDesc {
            get
            {
                if (DayNight == DayOrNight.Day)
                {
                    return "日";
                }
                else
                {
                    return "夜";
                }

            }
        
        }

        public Eye CheckEyeMode { get; set; }
    }
}
