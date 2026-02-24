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
        /// 云端检查ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public long CloudId { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public long CheckId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public long UserId { get; set; }

        /// <summary>
        /// 检查时间
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DateTime FitTime { get; set; }

        /// <summary>
        /// 检查时长
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int CheckDuration { get; set; } = 30;

        /// <summary>
        /// 检查距离
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public CheckDistance CheckDistance { get; set; }

        /// <summary>
        /// 光照状态
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public LightStatus LightStatus { get; set; }

        /// <summary>
        /// 眼别
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public Eye Eye { get; set; }

        /// <summary>
        /// 日夜
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public DayOrNight DayOrNight { get; set; }

        /// <summary>
        /// 瞳距
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int PupilDistance { get; set; }
        /// <summary>
        /// VA06
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int VA06 { get; set; }
        /// <summary>
        /// VA10
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int VA10 { get; set; }
        /// <summary>
        /// VA20
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int VA20 { get; set; }

        [SqlSugar.SugarColumn(IsNullable = true)]
        public int VA40 { get; set; }
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int VA60 { get; set; }

        [SqlSugar.SugarColumn(IsNullable = true)]
        public int VA80 { get; set; }



    }
}
