using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Entity
{
    /// <summary>
    /// 管理员
    /// </summary>
    public class Manger:RootEntity
    {
        public string name { get; set; }

        public string gender { get; set; }

        public int age { get; set; }

        public string phone { get; set; }

        public string type { get; set; }
    }
}
