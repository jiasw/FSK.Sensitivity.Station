using FSK.Sensitivity.Core.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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

    }
}
