using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    public class CheckInfo
    {
        public long DataId { get; set; }

        public long UserId { get; set; }
        public string ItemName {
            get
            {
                if (Type == CheckItem.CSF)
                {
                    return "对比敏感度";
                }
                else if (Type == CheckItem.DCK)
                {
                    return "暗环境适应";
                }
                return "";
            }
        }

        public CheckItem Type { get;set; }

        public string Result { get; set; }

        public string Status { get; set; }

        public DateTime CheckTime { get; set; }
    }

    /// <summary>
    /// 对比敏感度结果数据视图
    /// </summary>
    public class CheckResultDataView
    {
        public long CheckId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime FitTime { get; set; }

        /// <summary>
        /// 检查时长
        /// </summary>
        public int CheckDuration { get; set; } = 30;

        /// <summary>
        /// 检查距离
        /// </summary>
        public CheckDistance CheckDistance { get; set; }

        /// <summary>
        /// 光照状态
        /// </summary>
        public LightStatus LightStatus { get; set; }

        /// <summary>
        /// 眼别
        /// </summary>
        public Eye Eye { get; set; }

        /// <summary>
        /// 日夜
        /// </summary>
        public DayOrNight DayOrNight { get; set; }

        /// <summary>
        /// 瞳距
        /// </summary>
        public int PupilDistance { get; set; }

        /// <summary>
        /// VA06
        /// </summary>
        public int Left_VA06 { get; set; }
        /// <summary>
        /// VA10
        /// </summary>
        public int Left_VA10 { get; set; }
        /// <summary>
        /// VA20
        /// </summary>
        public int Left_VA20 { get; set; }

        public int Left_VA40 { get; set; }
        public int Left_VA60 { get; set; }

        public int Left_VA80 { get; set; }


        /// <summary>
        /// VA06
        /// </summary>
        public int Right_VA06 { get; set; }
        /// <summary>
        /// VA10
        /// </summary>
        public int Right_VA10 { get; set; }
        /// <summary>
        /// VA20
        /// </summary>
        public int Right_VA20 { get; set; }

        public int Right_VA40 { get; set; }
        public int Right_VA60 { get; set; }

        public int Right_VA80 { get; set; }



        public string Result
        {

            get { return "合格"; }
        }

    }

  
}
