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

        public const string HardWareTest = nameof(HardWareTest);

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
        public const string Main_Page_Report = "PrintReport";


        public const string Main_Page_Setting = "SystemSetting";

        public const string Main_Page_Setting_Store = "Store";
        public const string Main_Page_Setting_UserInfo = "UserInfo";
        public const string Main_Page_Setting_Patient = "Patients";
        public const string Main_Page_Setting_Netork = "NetWork";

        public const string Main_Page_CheckHistory = "CheckHistory";

        public const string Main_Page_TrainFrame = "TrainFrame";

        public const string Main_Dialog_ShowItems = "ShowItemsDialog";
        public const string Main_Dialog_Scan = "Scan";
        public const string Main_Dialog_Register = "Register";
        public const string Main_Dialog_Setting_UserInfo_Add = "UserInfoAdd";
        public const string Main_Dialog_AlertMsg = "AlertMsg";
        public const string Main_Dialog_Finish = "Finish";

        public const string ResultWindowsName = "CheckResult";

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
        /// 暗环境检查编码
        /// </summary>
        public const string CheckItemCode_Contrast = "277ebce5-cb73-4dc2-913d-aa60840e2904";

        /// <summary>
        /// 对比敏感度检查编码
        /// </summary>
        public const string CheckItemCode_Sensitivity = "2f83768f-0b37-43b4-a3df-357ae9ca1166";

        /// <summary>
        /// 训练完成标识
        /// </summary>
        public const string FinishItemKey= nameof(FinishItemKey);


        //等待硬件运动完成最长时间，单位：秒
        public const int WaitHardwareMotionTimeout = 12;

        /// <summary>
        /// csf训练左眼时长,单位：秒
        /// </summary>
        public const int LeftEyeDuration = 30;
        /// <summary>
        /// csf训练右眼时长,单位：秒
        /// </summary>
        public const int RightEyeDuration = 30;

        /// <summary>
        /// 暗环境适应等待时长,单位：秒
        /// </summary>
        public const int ContrastWaitDuration = 30;

        /// <summary>
        /// 暗环境适应检查时长,单位：秒
        /// </summary>
        public const int ContrastDuration = 30;



        /// <summary>
        /// 字典键值：门店名称
        /// </summary>
        public const string Dict_TypeCode_StoreName = "006";

        public const string Dict_TypeCode_StoreCode = "007";

        public const string Dict_TypeCode_StoreLogo = "008";

        public const string Dict_TypeCode_SerialPort = "001";
        #region 串口信息
        

        /// <summary>
        /// 字典键值：电机串口
        /// </summary>
        public const string Dict_Key_MotorPort = "02";

        #endregion

    }
}
