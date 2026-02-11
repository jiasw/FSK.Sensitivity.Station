using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using Microsoft.Extensions.Logging;
using Serilog;
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
using System.Windows.Input;

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

        Task HeartBeatAsync(string devicenum);

        /// <summary>
        /// 根据治疗方案id 获取检查、用户、医生信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task <PrescribeInfo> GetPrescribeInfoAsync(CloudSolutionDataItem item, string devicenum, string activeKey);

        
        Task<bool> ItemStart(DeviceData deviceData, ItemInfoDto itemInfoDto, string key);
        /// <summary>
        /// 项目结束
        /// </summary>
        /// <param name="item"></param>
        /// <param name="devicenum"></param>
        /// <returns></returns>
        Task<bool> ItemEnd(DeviceData deviceData, ItemStopDto<List<CheckResultDto>> itemStopDto, string key);

    }
    class PrescriptionInfo
    {
        public String PrescribeId { get; set; }

        public int  PrescribeType { get; set; }
    }

    public class ItemInfo
    {
        public string ItemId { get; set; }
        public string ItemCate { get; set; }
        public string ItemName { get; set; }
        /// <summary>
        /// 项目类型检查项目0，训练项目1
        /// </summary>
        public int ItemType { get; set; }
        public int PatientId { get; set; }
        public string ParientIdCard { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string PrescribeId { get; set; }
    }

    public class DeviceData
    {
        public string DeviceNum { get; set; }
        public WebAction Action { get; set; }
        public string Content { get; set; }

        
    }

    public class ItemInfoDto
    {
        public string PrescribeId { get; set; }
        public string ItemId { get; set; }
        public string ItemCate { get; set; }
        public string ItemName { get; set; }
        public int ItemType { get; set; }
        public string PatientId { get; set; }
        public string ItemRecordId { get; set; }
        public string ParientIdCard { get; set; }
        public string PatientName { get; set; }
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public DateTime StartTime { get; set; }
        public string RunParam { get; set; }
        public string Eye { get; set; }
    }


    public class ItemStopDto<T>
    {
        public string PrescribeId { get; set; }
        public int PrescribeType { get; set; }
        public string IdCard { get; set; }
        public string PatientId { get; set; }
        public string ItemID { get; set; }
        public string ItemRecordId { get; set; }
        public bool State { get; set; }
        public T Data { get; set; }
        public DateTime EndTime { get; set; }
        public string Eye { get; set; }
    }

    public class CheckResultDto
    {

        public string ItemID { get; set; }

        public int DoctorId { get; set; }
        /// <summary>
        /// 检查项目名
        /// </summary>
        public string ItemName { get; set; }
        public string EyeName { get; set; }
        /// <summary>
        /// 检查结果
        /// </summary>
        public string CheckResult { get; set; }
        

        /// <summary>
        /// 检查项目数据
        /// </summary>
        public string ItemData { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

    }


    /// <summary>
    /// 云端提交的暗环境结果
    /// </summary>
    public class CloudContrastResult
    {
        public DCKTime Time { get; set; }

        public DCKValue Value { get; set; }

    }

    public class CloudSensitivityResult
    {
        public Eye Eye { get; set; }

        public DayOrNight DayOrNight { get; set; }

        public CheckDistance  CheckDistance { get; set; }

        /// <summary>
        /// VA06
        /// </summary>
        public int VA06 { get; set; }
        /// <summary>
        /// VA10
        /// </summary>
        public int VA10 { get; set; }
        /// <summary>
        /// VA20
        /// </summary>
        public int VA20 { get; set; }

        public int VA40 { get; set; }
        public int VA60 { get; set; }

        public int VA80 { get; set; }
    }





    public class CloudSyncService : ICloudSyncService
    {
        private readonly HttpClient _remotehttpClient;
        private readonly HttpClient _localhttpClient;

        private readonly HttpClientConfig _config;
        private readonly ILogger<CloudSyncService> _logger;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly AppSetting _appSetting;
        public CloudSyncService(IContainerExtension containerExtension, IConfigurationService configurationService,ILogger<CloudSyncService> logger)
        {
            
            _logger = logger;
            _appSetting = configurationService.LoadSetting();
            _remotehttpClient = containerExtension.Resolve<HttpClient>("RemoteClient");
            _localhttpClient = containerExtension.Resolve<HttpClient>("LocalClient");
            _config = new HttpClientConfig()
            {
                TimeoutSeconds = 3000,
                RetryDelayMilliseconds = 1000,
                MaxRetryCount = 3,
            };
            // 配置HttpClient
            
            _remotehttpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);
            _localhttpClient.Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds);

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
            string url = _appSetting.RemoteServer;
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
                        var reply = await pingSender.SendPingAsync(ip);
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
                _remotehttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpcontent
                };
                var response = await SendWithRetryAsync(_remotehttpClient, request);
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
                _remotehttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpcontent
                };
                var response = await SendWithRetryAsync(_remotehttpClient,request);
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
        public async Task HeartBeatAsync(string devicenum)
        {
            try
            {
                string url = $"/api/v1/datasync/devicereport";
                string code = WebData.GetAESEncrypt(WebAction.CheckActive, new
                {
                    
                },devicenum).ToString();
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {"code",code}
                };
                HttpContent httpcontent = new FormUrlEncodedContent(dict);
                _localhttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpcontent
                };
                var response = await SendWithRetryAsync(_localhttpClient,request);
                var content = await response.Content.ReadAsStringAsync();
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"设备心跳请求失败: 错误: {ex.Message}");
            }
            
        }

        

        /// <summary>
        /// 根据治疗方案id 获取检查、用户、医生信息
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<PrescribeInfo> GetPrescribeInfoAsync(CloudSolutionDataItem item,string devicenum,string activeKey) {
            PrescribeInfo result = null;
            try
            {
                string url = $"/api/v1/DeviceReport/PostData";
                var prescriptionInfo = new PrescriptionInfo()
                {
                    PrescribeId=item.Guid,
                    PrescribeType=item.Type
                };
                string code = WebData.GetAESEncrypt(WebAction.QueryPrescribe, prescriptionInfo, devicenum,activeKey).ToString();
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {"code",code}
                };
                HttpContent httpcontent = new FormUrlEncodedContent(dict);
                _localhttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpcontent
                };
                var response = await SendWithRetryAsync(_localhttpClient,request);
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpResult<PrescribeResult> httpResult = JsonSerializer.Deserialize<HttpResult<PrescribeResult>>(content, _jsonOptions);
                    if (httpResult != null&&httpResult.Code==200&&httpResult.Data.Result) {
                        result = httpResult.Data.Data;
                        }
                    return result;
                }
                else
                {
                    return result;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"POST请求失败: 错误: {ex.Message}");
            }
            return result;

        }
        
        public async Task<bool> ItemStart(DeviceData deviceData, ItemInfoDto itemInfoDto,string key)
        {
            try
            {
                string url = $"/api/v1/DeviceReport/PostData";
                deviceData.Content = itemInfoDto.ToJson().AESEncrypt(key);
                string code = deviceData.ToJson().DesEncrypt();
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {"code",code}
                };
                HttpContent httpcontent = new FormUrlEncodedContent(dict);
                _localhttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpcontent
                };
                var response = await SendWithRetryAsync(_localhttpClient, request);
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpResult<ReportResult<bool>> httpResult = JsonSerializer.Deserialize<HttpResult<ReportResult<bool>>>(content, _jsonOptions);
                    if (httpResult != null)
                    {
                        return httpResult.Data.Result;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"项目开始失败: 项目名称: {itemInfoDto.ItemName},编码：{itemInfoDto.ItemId}: 错误: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// 项目结束
        /// </summary>
        /// <param name="item"></param>
        /// <param name="devicenum"></param>
        /// <returns></returns>
        public async Task<bool> ItemEnd(DeviceData deviceData, ItemStopDto<List<CheckResultDto>> itemStopDto, string key)
        {
            try
            {
                string url = $"/api/v1/DeviceReport/PostData";
                deviceData.Content = itemStopDto.ToJson().AESEncrypt(key);
                string code = deviceData.ToJson().DesEncrypt();
                Dictionary<string, string> dict = new Dictionary<string, string>()
                {
                    {"code",code}
                };
                HttpContent httpcontent = new FormUrlEncodedContent(dict);
                _localhttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                var request = new HttpRequestMessage(HttpMethod.Post, url)
                {
                    Content = httpcontent
                };
                var response = await SendWithRetryAsync(_localhttpClient, request);
                var content = await response.Content.ReadAsStringAsync();
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    HttpResult<ReportResult<bool>> httpResult = JsonSerializer.Deserialize<HttpResult<ReportResult<bool>>>(content, _jsonOptions);
                    if (httpResult != null)
                    {
                        return httpResult.Data.Result;
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"项目结束失败: 项目名称: {itemStopDto.Data.First().ItemName},编码：{itemStopDto.Data.First().ItemID}: 错误: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 带重试的发送请求
        /// </summary>
        private async Task<HttpResponseMessage> SendWithRetryAsync(HttpClient httpClient, HttpRequestMessage request)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    var response = await httpClient.SendAsync(request);
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
