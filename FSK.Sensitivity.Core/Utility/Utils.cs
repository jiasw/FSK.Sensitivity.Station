using FSK.Sensitivity.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Yitter.IdGenerator;

namespace FSK.Sensitivity.Core.Utility
{
    public class Utils
    {
        public static List<ShowItemsModel> GetEnumDisplayList<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T))
                .Cast<T>()
                .Select(e => new ShowItemsModel
                {
                    Value = e.ToString(),
                    Name = e.GetType()
                        .GetField(e.ToString())
                        .GetCustomAttributes<DescriptionAttribute>(false)
                        .FirstOrDefault()?.Description ?? e.ToString()
                }).ToList();
        }

        /// <summary>
        /// 启动应用程序
        /// </summary>
        /// <param name="processName">应用程序全路径</param>
        public static void StartProcess(string processName)
        {
            

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = processName,
                UseShellExecute = true // 设置为true可以使用操作系统的Shell来启动进程
            };

            // 创建一个新的Process对象
            Process process = new Process
            {
                StartInfo = startInfo
            };

            try
            {
                // 启动进程
                process.Start();
                Console.WriteLine("应用程序已成功启动。");
            }
            catch (Exception ex)
            {
                // 捕获并显示任何可能发生的异常
                Console.WriteLine("启动应用程序时出错: " + ex.Message);
            }

        }

        /// <summary>
        /// 关机
        /// </summary>
        public static void ShutDown()
        {
            using var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "shutdown";
            process.StartInfo.Arguments = "/s /t 0"; // /s关机, /t 0延迟0秒
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
        }
        /// <summary>
        /// 重启设备
        /// </summary>
        public static void Restart()
        {
            using var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = "shutdown";
            process.StartInfo.Arguments = "/r /t 0"; // /r重启, /t 0延迟0秒
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
        }

        /// <summary>
        /// 检查内部网络连接状态
        /// </summary>
        /// <returns></returns>
        public static bool CheckInternalNetWorkStatus(string host = "www.baidu.com")
        {
            Ping ping = new Ping();
            try
            {
                PingReply reply = ping.Send(host);
                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }

        }



        /// 生成一个区间的随机数，并排除指定的数字
        public static int GenerateRandomNumber(int start, int end, params int[] excludeNumbers)
        {
            Random random = new Random();
            int result = random.Next(start, end);
            while (excludeNumbers.Contains(result))
            {
                result = random.Next(start, end);
            }
            return result;
        }

        /// <summary>
        /// 生成雪花ID
        /// </summary>
        /// <returns></returns>
        public static long GenerateSnowID()
        {
            var options = new IdGeneratorOptions(1); // 1 是机器码，分布式环境下需唯一
            YitIdHelper.SetIdGenerator(options);
            return YitIdHelper.NextId();
        }

    }
}
