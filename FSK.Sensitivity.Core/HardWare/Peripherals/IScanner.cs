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

       // public event ScanerDelegate ScanCompleted;

        /// <summary>
        /// 是否可用
        /// </summary>

        bool IsAvailable { get; }

        void Start();

        void Stop();
    }
}
