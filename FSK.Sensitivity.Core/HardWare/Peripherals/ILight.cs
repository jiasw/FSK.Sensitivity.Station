using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    public interface ILight
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        bool IsAvailable { get; }
        /// <summary>
        /// 打开左边灯光
        /// </summary>
        Task<bool> TurnOnLeft();

        /// <summary>
        /// 打开右边灯光
        /// </summary>
        Task<bool> TurnOnRight();

        /// <summary>
        /// 关闭左边灯光
        /// </summary>
        Task<bool> TurnOffLeft();

        /// <summary>
        /// 关闭右边灯光
        /// </summary>
        Task<bool> TurnOffRight();

        /// <summary>
        /// 打开所有灯光
        /// </summary>
        Task<bool> TurnOnAll();

        /// <summary>
        /// 关闭所有灯光
        /// </summary>
        Task<bool> TurnOffAll();

    }
}
