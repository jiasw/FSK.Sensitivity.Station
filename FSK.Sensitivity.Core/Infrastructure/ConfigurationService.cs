using FSK.Sensitivity.Core.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Infrastructure
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly string _configPath;
        private readonly JsonSerializerOptions _jsonOptions;
        private AppSetting _setting;
        public ConfigurationService()
        {
            // 配置文件路径
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resource", "AppSetting.dll");

            // JSON序列化选项
            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.Create(UnicodeRanges.All),
                PropertyNameCaseInsensitive = true,
                Converters =
                    {
                        new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: false)
                    }
            };
            // 确保配置文件目录存在
            var configDir = Path.GetDirectoryName(_configPath);
            if (!Directory.Exists(configDir))
            {
                Directory.CreateDirectory(configDir);
            }
        }

        public AppSetting LoadSetting()
        {
            if (!File.Exists(_configPath))
            {
                throw new FileNotFoundException("配置文件未找到", _configPath);
            }
            string json =  File.ReadAllText(_configPath);
            Console.WriteLine($"读取配置文件: {json}");
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new FileNotFoundException("配置文件为空未找到", _configPath);
            }
            if(_setting != null)
            {
                return _setting;
            }
            var setting = System.Text.Json.JsonSerializer.Deserialize<AppSetting>(json, _jsonOptions);
            _setting= setting ;
            return setting;
        }
        public void SaveSetting(AppSetting setting)
        {
            try
            {
                if (setting == null)
                    throw new ArgumentNullException(nameof(setting));
                var json = System.Text.Json.JsonSerializer.Serialize(setting, _jsonOptions);
                 File.WriteAllText(_configPath, json);
                Console.WriteLine($"保存配置文件: {json}");
                _setting = null; // 清除缓存以确保下次加载时读取最新配置
            }
            catch (Exception ex)
            {
                // 记录日志或处理异常
                Console.WriteLine($"保存配置文件失败: {ex.Message}");
                throw;
            }
        }
        
    }
}
