using FSK.Sensitivity.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    /// <summary>
    /// 敏感度配置参数
    /// </summary>
    public class SensitivityConfigParam: BindableBase
    {
        private int _checkDuration;
        
       
        /// <summary>
        /// 检查时长（秒）
        /// </summary>
        public int CheckDuration 
        {
            get { return _checkDuration; }
            set { SetProperty(ref _checkDuration, value); }
        }

        // 检查距离
        private CheckDistance _checkDistance;
        public CheckDistance CheckDistance
        {
            get => _checkDistance;
            set
            {
                if (SetProperty(ref _checkDistance, value))
                {
                    RaisePropertyChanged(nameof(CheckDistanceDisplay));
                }
            }
        }
        public string CheckDistanceDisplay => EnumExtensions.GetDescription(CheckDistance);

        // 灯光开关
        private LightStatus _isLightOn;
        public LightStatus IsLightOn  
        {
            get => _isLightOn;
            set
            {
                if (SetProperty(ref _isLightOn, value))
                {
                    RaisePropertyChanged(nameof(IsLightOnDisplay));
                }
            }
        }
        public bool IsLightOnDisplay => IsLightOn == LightStatus.Strong;

        private Eye _eyes;
        public Eye Eyes
        {
            get => _eyes;
            set
            {
                if (SetProperty(ref _eyes, value))
                {
                    RaisePropertyChanged(nameof(EyesDisplay));
                }
            }
        }
        public string EyesDisplay => Eyes.GetDescription();



        private DayOrNight _dayNight;
        public DayOrNight DayNight
        {
            get => _dayNight;
            set
            {
                if (SetProperty(ref _dayNight, value))
                {
                    // 当前属性更改时通知显示属性
                    RaisePropertyChanged(nameof(DayNightDisplay));
                }
            }
        }
        // 动态获取最新值，无需缓存
        public string DayNightDisplay => DayNight.GetDescription();
       


        private int _pd;
        /// <summary>
        /// 瞳距
        /// </summary>
        public int PD
        {
            get { return _pd; }
            set { SetProperty(ref _pd, value); }
        }

    }
}
