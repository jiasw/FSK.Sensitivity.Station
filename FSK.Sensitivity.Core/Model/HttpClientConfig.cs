using FSK.Sensitivity.Core.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Model
{
    /// <summary>
    /// HTTP客户端配置
    /// </summary>
    public class HttpClientConfig
    {
        /// <summary>
        /// 基地址
        /// </summary>
        public string BaseAddress { get; set; }
        /// <summary>
        /// 请求超时时间（秒）
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
        /// <summary>
        /// 重试次数
        /// </summary>
        public int MaxRetryCount { get; set; } = 3;
        /// <summary>
        /// 重试延迟（毫秒）
        /// </summary>
        public int RetryDelayMilliseconds { get; set; } = 1000;
    }

    public enum WebAction
    {
        None = 0000,
        Heart = 1009,
        CheckActive = 1002,
        RegisDevice = 1001,
        CreateUser = 1019,
        GetStore = 2001,
        Patient = 5001,
        QueryPrescribe = 6001,
        ItemStart = 3001,
        ItemEnd = 3002,
        DeviceError = 4004,
        ValidItems = 6009,
        DoctorLogin = 7021,
    }


    public class WebData
    {
        public string DeviceNum { get; set; }
        public WebAction Action { get; set; }
        public string Content { get; set; }


        public static WebData GetDESEncrypt(WebAction action, object obj)
        {
            WebData webData = new WebData();
            webData.Action = action;
            webData.Content=obj.ToJson().DesEncrypt();
            return webData;
        }
        public static WebData GetAESEncrypt(WebAction action, object obj,string deviceNum)
        {
            WebData webData = new WebData();
            webData.Action = action;
            webData.DeviceNum = deviceNum;
            webData.Content = obj.ToJson().AESEncrypt();
            return webData;
        }

        public static WebData GetAESEncrypt(WebAction action, object obj, string deviceNum,string activeKey)
        {
            WebData webData = new WebData();
            webData.Action = action;
            webData.DeviceNum = deviceNum;
            webData.Content = obj.ToJson().AESEncrypt(activeKey.DesDecrypt());
            return webData;
        }
        public static WebData GetAESEncrypt(WebAction action, object obj)
        {
            WebData webData = new WebData();
            webData.Action = action;
            webData.Content = obj.ToJson().AESEncrypt();
            return webData;
        }

        public override string ToString()
        {
            return this.ToJson().DesEncrypt();
        }
    }

    public class DeviceRegistResult
    {
        public bool Result { get; set; }
        public string DeviceNum { get; set; }
    }

    public class DeviceActiveResult
    {
        public bool Result { get; set; }

        public string Message { get; set; }
        public string StoreID { get; set; }
        public string StoreName { get; set; }
        public string Address { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string StoreProvince { get; set; }
        public string StoreCity { get; set; }
        public string StoreDistrict { get; set; }
        public string DataSecretKey { get; set; }
        public string ActiveDate { get; set; }
    }


    public class HttpResultBase
    {
        public int Code { get; set; }

        public string Msg { get; set; }
    }

    public class HttpResult<T> : HttpResultBase
    {
        public T Data { get; set; }
    }

    public class PrescribeResult
    {
        public bool Result { get; set; }

        public PrescribeInfo Data {  get; set; }
    }

    /// <summary>
    /// HTTP请求异常
    /// </summary>
    public class HttpRequestException : Exception
    {
        public int? StatusCode { get; set; }
        public string ResponseContent { get; set; }
        public HttpRequestException(string message, int? statusCode = null, string responseContent = null)
            : base(message)
        {
            StatusCode = statusCode;
            ResponseContent = responseContent;
        }
    }
}
