using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class AppSetting
    {
        public string SqlLiteDb { get; set; }

        public string RemoteSupportPath { get; set; }
        /// <summary>
        /// 手柄串口配置
        /// </summary>
        public SerialPortConfig HandlerConfig { get; set; }

        /// <summary>
        /// 电机串口配置
        /// </summary>
        public SerialPortConfig MotorConfig { get; set; }

        /// <summary>
        /// 灯光串口
        /// </summary>
        public SerialPortConfig LightConfig { get; set; }

        /// <summary>
        /// 扫码器串口配置
        /// </summary>
        public SerialPortConfig ScannerConfig { get; set; }
    }
}
