using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Main.Controls;
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
        public bool IsLogin { get; set; }

        
        /// <summary>
        /// 弹窗服务
        /// </summary>
        public IDialogService DialogService { get; set; }


        public Window MainWindow { get; set; }

    }
}
