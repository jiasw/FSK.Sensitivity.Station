using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    public interface IMotor
    {
        bool IsAvailable { get; }
        /// <summary>
        /// 初始化丝杆，转盘位置
        /// </summary>
        /// <returns></returns>
        Task<bool> Initialize();
        /// <summary>
        /// 设置两个转盘的间距
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Task<bool> SetSlideBlock(int position);

        ///<summary>
        ///设置左侧转盘位置
        ///</summary>
        ///<param name="position"></param>
        Task<bool> SetLeftDisk(int position);

        /// <summary>
        /// 设置右侧转盘位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Task<bool> SetRightDisk(int position);

    }
}
