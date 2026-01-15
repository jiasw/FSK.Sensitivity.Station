using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    public class ContrastConfigParam : BindableBase
    {
        private DCKTime _checkDuration;


        /// <summary>
        /// 检查时长（秒）
        /// </summary>
        public DCKTime CheckDuration
        {
            get { return _checkDuration; }
            set { SetProperty(ref _checkDuration, value); }
        }

        private int _pd;
        /// <summary>
        /// 瞳距
        /// </summary>
        public int PD
        {
            get { return _pd; }
            set { SetProperty(ref _pd, value); }
        }

        /// <summary>
        /// 检查前的等待时间（秒）
        /// </summary>
        public int WaitTime { get => 30; }
    }
}
