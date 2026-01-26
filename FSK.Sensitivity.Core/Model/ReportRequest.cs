using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    public class ReportRequest
    {
        public ReportRequest() { }


        public long DataID { get; set; }

        public CheckItem DataType { get; set; }

        /// <summary>
        /// 跳转过来的界面：0：对比敏感度训练界面；1：暗环境适应训练界面；2：其他
        /// </summary>
        public int FromPage { get; set; }


        public string Date { get; set; }

    }
}
