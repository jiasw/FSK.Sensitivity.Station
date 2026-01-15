using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    public interface Iwifi
    {
        /// <summary>
        /// 获取可用的WIFI网络
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetAvailableNetworks();

        /// <summary>
        /// 连接WIFI网络
        /// </summary>
        /// <returns></returns>
        bool Connect(string name,string password, Action<bool> onConnectComplete);

    }
}
