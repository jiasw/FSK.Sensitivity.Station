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

        public StoreInfo StoreInfo { get; set; }

        /// <summary>
        /// Modbus串口名称
        /// </summary>
        public string ModbusPortName { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ComfirmPassword { get; set; }


    }
}
