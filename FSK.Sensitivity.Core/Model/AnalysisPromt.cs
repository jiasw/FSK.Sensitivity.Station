using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    public class AnalysisPromt
    {
        public string Eyes { get; set; }

        public string light {  get; set; }

        public string mode { get; set; }

        public string distance { get; set; }

        public AnalysisData Data { get; set; }
    }

    public class AnalysisData
    {
        public string VA1 { get; set; }
        public string VA2 { get; set; }
        public string VA3 { get; set; }
        public string VA4 { get; set; }
        public string VA5 { get; set; }
        public string VA6 { get; set; }
    }

}
