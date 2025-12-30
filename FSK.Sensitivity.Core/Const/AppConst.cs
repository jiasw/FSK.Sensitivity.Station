using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Const
{
    public class AppConst
    {
        /// <summary>
        /// 内容区域
        /// </summary>
        public  const  string MainRegion = nameof(MainRegion);

        /// <summary>
        /// 检查内容区域
        /// </summary>
        public const string TrainRegion = nameof(TrainRegion);

        /// <summary>
        /// 主屏菜单界面
        /// </summary>
        public const string  Main_Page_Menu= "MainMenu";

        public const string  Main_Page_Login = "Login";


        public const string  Main_Page_SensitivitySetting = "SensitivityConfig";
        public const string Main_Page_SensitivityTraining = "SensitivityTraining";

        public const string Main_Page_ContrastSetting = "ContrastConfig";
        public const string Main_Page_ContrastTraining = "ContrastTraining";
        public const string Main_Page_SerialPortConfig = "SerialPortConfig";
        

        public const string Main_Page_Setting = "SystemSetting";

        public const string Main_Page_Setting_Store = "Store";
        public const string Main_Page_Setting_UserInfo = "UserInfo";
        public const string Main_Page_Setting_Patient = "Patients";

        public const string Main_Page_CheckHistory = "CheckHistory";

        public const string Main_Page_TrainFrame = "TrainFrame";

        public const string Main_Dialog_ShowItems = "ShowItemsDialog";
        public const string Main_Dialog_Scan = "Scan";
        public const string Main_Dialog_Register = "Register";
        public const string Main_Dialog_Setting_UserInfo_Add = "UserInfoAdd";
        public const string Main_Dialog_AlertMsg = "AlertMsg";
        /// <summary>
        /// 副屏视标区域
        /// </summary>
        public const string SignRegion = nameof(SignRegion);

        /// <summary>
        /// 副屏默认界面
        /// </summary>
        public const string Sign_Page_Init = "SecondaryInit";

        public const string Sign_Page_Contrast = "SecondaryContrast";

        public const string Sign_Page_Sensitivity = "SecondarySensitivity";
        
        /// <summary>
        /// csf训练左眼时长,单位：秒
        /// </summary>
        public const int LeftEyeDruation = 30;
        /// <summary>
        /// csf训练右眼时长,单位：秒
        /// </summary>
        public const int RightEyeDruation = 30;

        

        /// <summary>
        /// 字典键值：门店名称
        /// </summary>
        public const string Dict_TypeCode_Store = "006";

        public const string Dict_TypeCode_SerialPort = "001";
        #region 串口信息
        

        /// <summary>
        /// 字典键值：电机串口
        /// </summary>
        public const string Dict_Key_MotorPort = "02";

        /// <summary>
        /// 字典键值：扫描仪串口
        /// </summary>
        public const string Dict_Key_ScannerPort = "04";
        #endregion

    }
}
