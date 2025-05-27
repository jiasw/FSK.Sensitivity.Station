using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    /// <summary>
    /// 手柄
    /// </summary>
    public interface IHandle
    {
        bool IsAvailable { get; }
        void UP();
        void DOWN();
        void LEFT();
        void RIGHT();
    }
}
