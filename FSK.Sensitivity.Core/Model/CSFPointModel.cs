using Dm;
using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    /// <summary>
    /// 暗环境适应度检测点
    /// </summary>
    public class DCKPointModel
    {
        public int XIndex
        {

            get
            {
                return (int)DCKTime - 1;
            }
        }
        public int YIndex
        {
            get
            {
                return (int)DCKValue - 1;
            }
        }


        public DCKTime DCKTime { get; set; }

        public DCKValue DCKValue { get; set; }
    }


    public class CSFPointModel
    {
        public int XIndex
        {

            get
            {
                return (int)Value - 1;
            }
        }
        public int YIndex
        {

            get
            {
                return (int)CS - 1;
            }
        }


        public CSFVA Value { get; set; }

        public CSFValue CS { get; set; }
    }
}
