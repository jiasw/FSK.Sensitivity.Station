using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    public class SerialPortConfig
    {
        /// <summary>
        /// 串口名称
        /// </summary>
        public string SerialPortName { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int BaudRate { get; set; } = 115200;

        /// <summary>
        /// 奇偶校验位
        /// </summary>
        public Parity Parity { get; set; } = Parity.None;
        /// <summary>
        /// 停止位
        /// </summary>
        public StopBits StopBits { get; set; } = StopBits.One;
        /// <summary>
        /// 数据长度
        /// </summary>
        public int DataBits { get; set; } = 8;
    }
}
