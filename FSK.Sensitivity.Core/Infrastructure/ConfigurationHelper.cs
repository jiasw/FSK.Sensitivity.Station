using FSK.Sensitivity.Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Infrastructure
{
    public class ConfigurationHelper
    {
        private  AppSetting _appSetting;

        string _appSettingFilePath = "";

        public ConfigurationHelper()
        {
            _appSettingFilePath= Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "AppSetting.dll");
            
        }

        

        public  AppSetting GetAppConfiguration()
        {
            return _appSetting;
        }

        public  void BuildAppConfiguration()
        {
            _appSetting = JsonConvert.DeserializeObject<AppSetting>(File.ReadAllText(_appSettingFilePath));
        }

        public  void SaveAppConfiguration(AppSetting appSetting)
        {
            string jsonString = JsonConvert.SerializeObject(appSetting, Formatting.Indented);
            File.WriteAllText(_appSettingFilePath, jsonString);
        }
        
    }
}
