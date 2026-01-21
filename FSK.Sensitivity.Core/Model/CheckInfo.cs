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


}
