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

        public int PD { get; set; }

        public LightStatus Light { get; set; }

        public string LightDesc {
            get
            {
                if (Light == LightStatus.Off)
                {
                    return "关";
                }
                else
                {
                    return "开";
                }
            }
        }

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

        public CheckDistance checkDistance { get; set; }


        /// <summary>
        /// 左眼检查用时
        /// </summary>
        public int LeftEyeDuration { get; set; } = 0;

        /// <summary>
        /// 右眼检查用时
        /// </summary>
        public int RightEyeDuration { get; set; } = 0;

        /// <summary>
        /// 当前检查用时
        /// </summary>
        public int CurrentDuration {
            get
            {
                if (this.CheckEyeMode == Eye.OS)
                {
                    return this.LeftEyeDuration;
                }else if (this.CheckEyeMode==Eye.OD)
                {
                    return this.RightEyeDuration;
                }
                return 0;
            }
        } 

        /// <summary>
        /// 总用时
        /// </summary>
        public int TotalDuration
        {
            get { return LeftEyeDuration + RightEyeDuration; }
        }
    }
}
