using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    /// <summary>
    /// 扫码器接口
    /// </summary>
    public interface IScanner
    {
        /// <summary>
        /// 是否可用
        /// </summary>

        bool IsAvailable { get; }

        string Read();
    }
}
