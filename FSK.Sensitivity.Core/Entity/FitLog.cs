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
        public long CloudId { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// 检查时间
        /// </summary>
        public DateTime FitTime { get; set; }

        /// <summary>
        /// 适应时长
        /// </summary>
        public int FitDrution { get; set; }

        /// <summary>
        /// 适应度 
        /// </summary>
        public int FitResult { get; set; }
    }
}
