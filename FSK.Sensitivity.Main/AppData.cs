using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Main.Controls;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FSK.Sensitivity.Main
{
    public class AppData
    {
        private static readonly AppData _instance = new AppData();
        private AppData()
        {
        }
        // 公共静态属性，提供全局访问点
        public static AppData Instance
        {
            get
            {
                return _instance;
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public void Logout()
        {
            this.CurrentManger = null;
            this.IsLogin = false;
        }

        /// <summary>
        /// 当前患者
        /// </summary>
        public Patient  CurrentPatient { get; set; }

        /// <summary>
        /// 当前管理员
        /// </summary>
        public Manger CurrentManger { get; set; }
       
       
        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogin { get; set; }=false;


        /// <summary>
        /// 当前设备是否注册
        /// </summary>
        public bool IsRegister { get; set; }=false;

        /// <summary>
        /// 当前设备是否激活
        /// </summary>
        public bool IsActivate { get; set; }= false;

        /// <summary>
        /// 硬件是否可用
        /// </summary>
        public bool HardwareAvailable { get; set; }=false;

        /// <summary>
        /// 是否连接云端
        /// </summary>
        public bool IsConnectCloud { get; set; }= false;


        /// <summary>
        /// 设备激活信息
        /// </summary>
        public DeviceActiveResult DeviceActiveResult { get; set; }



        /// <summary>
        /// 当前设备运行模式
        /// </summary>
        public DeviceRunMode DeviceRunMode { get; set; }=DeviceRunMode.NETWORKED;

        /// <summary>
        /// 设备编码
        /// </summary>
        public string DeviceNo { get; set; }

        /// <summary>
        /// 处方信息
        /// </summary>
        public PrescribeInfo PrescribeInfo { get; set; }


    }
}
