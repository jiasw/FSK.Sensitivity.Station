using FSK.Sensitivity.Core.HardWare.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    public interface IMotor
    {
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
        Task<bool> SetSlideBlock(short position);

        /// <summary>
        /// 丝杆是否在移动
        /// </summary>
        /// <returns></returns>
        Task<bool> IsSlideMove();

        ///<summary>
        ///设置左侧转盘位置
        ///</summary>
        ///<param name="position"></param>
        Task<bool> SetLeftDisk(short position);

        /// <summary>
        /// 设置右侧转盘位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        Task<bool> SetRightDisk(short position);


        /// <summary>
        /// 判断做转盘是否在运动
        /// </summary>
        /// <returns></returns>
        Task<bool> IsLeftMove();
        /// <summary>
        /// 判断右转盘是否在运动
        /// </summary>
        /// <returns></returns>
        Task<bool> IsRightMove();
    }
}
