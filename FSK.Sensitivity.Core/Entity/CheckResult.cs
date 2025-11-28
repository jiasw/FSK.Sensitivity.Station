using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Entity
{

    public class CheckResult: RootEntity
    {
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
        public CheckDistance CeckDistance { get; set; }

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
        public int VA06 { get; set; }
        /// <summary>
        /// VA10
        /// </summary>
        public int VA10 { get; set; }
        /// <summary>
        /// VA20
        /// </summary>
        public int VA20 { get; set; }

        public int VA40 { get; set; }
        public int VA60 { get; set; }

        public int VA80 { get; set; }



    }
}
