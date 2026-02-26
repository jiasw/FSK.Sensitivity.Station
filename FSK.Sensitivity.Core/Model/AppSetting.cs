using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
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

        /// <summary>
        /// 远程后台接口
        /// </summary>
        public string RemoteServer { get; set; }

        /// <summary>
        /// 本地后台接口
        /// </summary>
        public string LocalServer { get; set; }

        /// <summary>
        /// Web分析服务地址
        /// </summary>
        public string WebAnalysisServer { get; set; }

        public DeviceRunMode DeviceRunMode { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public DeviceInfo DeviceInfo { get; set; }
    }

    public class DeviceInfo
    {

        /// <summary>
        /// 主程序名称
        /// </summary>
        public string MainProgramExeName { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 设备类型
        /// </summary>
        public string DeviceType {  get; set; }

        /// <summary>
        /// 设备型号
        /// </summary>
        public string DeviceModel { get; set; }
        /// <summary>
        /// 设备编号
        /// </summary>
        public string DeviceNo { get; set; }
        /// <summary>
        /// 程序版本号
        /// </summary>
        public string ProductVersion { get; set; }
        /// <summary>
        /// 程序更新时间
        /// </summary>
        public string UpdateTime { get; set; }
        /// <summary>
        /// 更新等待时间
        /// </summary>
        public int UpdateWaitTime { get; set; } = 10;

        /// <summary>
        /// 工厂名称
        /// </summary>
        public string FactoryName {  get; set; }

        /// <summary>
        /// 工厂电话
        /// </summary>
        public string FactoryPhone { get; set; }

    }

    }
