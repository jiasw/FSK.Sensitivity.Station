using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Security.Policy;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace FSK.Sensitivity.Core.Infrastructure
{
    /// <summary>
    /// HTTP服务接口
    /// </summary>
    public interface ICloudSyncService
    {
        /// <summary>
        /// 服务器是否可以连接
        /// </summary>
        /// <returns></returns>
        Task<bool> IsCanConnect();

        /// <summary>
        /// 注册设备
        /// </summary>
        Task<DeviceRegistResult> RegisterDevice();


        Task<DeviceActiveResult> GetActiveResultAsync(string devicenum);
    }

    public class CloudSyncService : ICloudSyncService
    {
        private readonly HttpClient _httpClient;
        private readonly HttpClientConfig _config;
        private readonly ILogger<CloudSyncService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly AppSetting _appSetting;
        public CloudSyncService(HttpClient httpClient ,IConfigurationService configurationService,ILogger<CloudSyncService> logger)
        {
            _logger = logger;
            _appSetting = configurationService.LoadSetting();
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _config = new HttpClientConfig()
            {
                TimeoutSeconds = 3000,
                RetryDelayMilliseconds = 1000,
                MaxRetryCount = 3,
                BaseAddress= _appSetting.RegisterDomain

            };
            // 配置HttpClient
            _httpClient.BaseAddress = new Uri(_config.BaseAddress);
            _httpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);
            
            // JSON序列化选项
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }


        public async Task<bool> IsCanConnect()
        {
            bool flag = false;
            string url = _appSetting.RegisterDomain;
            if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
            {
                string ip = uri.Host;
                int port = uri.Port;
                if (string.IsNullOrWhiteSpace(ip))
                {
                    return false;
                }
                using (Ping pingSender = new Ping())
                {
                    try
                    {
                        var reply = await pingSender.SendPingAsync(_appSetting.RegisterDomain);
                        flag = reply != null && reply.Status == IPStatus.Success;
                    }
                    catch { }
                }
            }
            
            return flag;
        }

        /// <summary>
        /// 注册设备
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public async Task<DeviceRegistResult> RegisterDevice()
        {
            try
            {
                string url = $"/api/v1/datasync/devicereport";
                string code =WebData.GetDESEncrypt(WebAction.RegisDevice, new
                {
                    DeviceType = _appSetting.DeviceInfo.DeviceType,
                    DeviceModel = _appSetting.DeviceInfo.DeviceModel,
                    ProductName = _appSetting.DeviceInfo.ProductName,
                    FactoryName = _appSetting.DeviceInfo.FactoryName,
                    FactoryPhone = _appSetting.DeviceInfo.FactoryPhone
                }).ToString();
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {"code",code}
                };
                HttpContent httpcontent = new FormUrlEncodedContent(dict);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpcontent
                };
                var response = await SendWithRetryAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpResult<DeviceRegistResult> httpResult = JsonSerializer.Deserialize<HttpResult<DeviceRegistResult>>(content, _jsonOptions);
                    if(httpResult!=null&&httpResult.Code == 200)
                    {
                        return httpResult.Data;
                    }
                    else
                    {
                        return new DeviceRegistResult()
                        {
                            DeviceNum = "",
                            Result = false
                        };
                    }
                }
                else
                {
                    return new DeviceRegistResult()
                    {
                        DeviceNum = "",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST请求失败: 错误: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 获取设备激活结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<DeviceActiveResult> GetActiveResultAsync(string devicenum)
        {
            try
            {
                string url = $"/api/v1/datasync/devicereport";
                string code = WebData.GetAESEncrypt(WebAction.CheckActive, new
                {
                    DeviceNum = devicenum,
                },devicenum).ToString();
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {"code",code}
                };
                HttpContent httpcontent = new FormUrlEncodedContent(dict);
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpcontent
                };
                var response = await SendWithRetryAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpResult<DeviceActiveResult> httpResult = JsonSerializer.Deserialize<HttpResult<DeviceActiveResult>>(content, _jsonOptions);
                    if (httpResult != null && httpResult.Code == 200)
                    {
                        DeviceActiveResult deviceresult= httpResult.Data;
                        if (deviceresult != null&&deviceresult.Result) {

                            return deviceresult;
                        }
                        else
                        {
                            return new DeviceActiveResult()
                            {
                                Result = false
                            };
                        }
                    }
                    else
                    {
                        return new DeviceActiveResult()
                        {
                            Result = false
                        };
                    }
                }
                else
                {
                    return new DeviceActiveResult()
                    {
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST请求失败: 错误: {ex.Message}");
            }
            return new DeviceActiveResult()
            {
                Result = false
            };
        }

        /// <summary>
        /// 心跳
        /// </summary>
        /// <returns></returns>
        public async Task HeartBeatAsync()
        {

        }



        /// <summary>
        /// 带重试的发送请求
        /// </summary>
        private async Task<HttpResponseMessage> SendWithRetryAsync(HttpRequestMessage request)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    var response = await _httpClient.SendAsync(request);
                    return response;
                }
                catch (Model.HttpRequestException ex) when (retryCount < _config.MaxRetryCount)
                {
                    retryCount++;
                    _logger.LogWarning($"请求失败，准备重试 ({retryCount}/{_config.MaxRetryCount}): {ex.Message}");
                    await Task.Delay(_config.RetryDelayMilliseconds);
                }
                catch (TaskCanceledException ex) when (retryCount < _config.MaxRetryCount)
                {
                    retryCount++;
                    _logger.LogWarning($"请求超时，准备重试 ({retryCount}/{_config.MaxRetryCount}): {ex.Message}");
                    await Task.Delay(_config.RetryDelayMilliseconds);
                }
            }
        }
    }
}
