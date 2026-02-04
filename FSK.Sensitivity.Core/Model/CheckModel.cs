using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    public class CheckModel
    {
        /// <summary>
        /// 项目编码
        /// </summary>
        public string ItemGuid { get; set; } = "";

        /// <summary>
        /// 项目参数
        /// </summary>
        public string Payload { get; set; } = "";
        /// <summary>
        /// 是否检查完成
        /// </summary>
        public bool IsChecked { get; set; } = false;
        /// <summary>
        /// 检查结果
        /// </summary>
        public string Result { get; set; } = "";
    }
}
