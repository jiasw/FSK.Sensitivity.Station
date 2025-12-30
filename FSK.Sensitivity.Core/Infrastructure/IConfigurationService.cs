using FSK.Sensitivity.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FSK.Sensitivity.Core.Infrastructure
{
    public interface IConfigurationService
    {
        /// <summary>
        /// 加载配置
        /// </summary>
        /// <returns></returns>
        AppSetting LoadSetting();
        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        void SaveSetting(AppSetting setting);
        
    }
}
