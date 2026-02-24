using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Entity
{
    public class FitLog: RootEntity
    {
        /// <summary>
        /// 云端检查ID
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public long CloudId { get; set; }
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
        /// 适应时长
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int FitDrution { get; set; }

        /// <summary>
        /// 适应度 
        /// </summary>
        [SqlSugar.SugarColumn(IsNullable = true)]
        public int FitResult { get; set; }
    }
}
