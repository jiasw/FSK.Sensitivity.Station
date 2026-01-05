using FSK.Sensitivity.Core.HardWare.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Drivers
{
    /// <summary>
    /// 转盘、丝杆电机控制器
    /// </summary>
    public class MotorController : IMotor
    {
        private readonly IModbusService modbusService;

        public MotorController(IModbusService modbusService)
        {
            this.modbusService = modbusService;
        }


        /// <summary>
        /// 初始化丝杆，转盘电机位置
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Initialize()
        {
           return await modbusService.WriteSingleRegisterAsync(3, 1);
        }
        /// <summary>
        /// 设置左转盘位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<bool> SetLeftDisk(short position)
        {
            if (position < 0 || position > 5)
            {
                return false;
            }
            return await modbusService.WriteSingleRegisterAsync(6, position);
        }
        /// <summary>
        /// 设置右转盘位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<bool> SetRightDisk(short position)
        {
            if (position < 0 || position > 5)
            {
                return false;
            }
            return await modbusService.WriteSingleRegisterAsync(7, position);
        }
        /// <summary>
        /// 设置丝杆位置
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task<bool> SetSlideBlock(short position)
        {
            
            return await modbusService.WriteSingleRegisterAsync(8, position);

        }
        /// <summary>
        /// 判断丝杆是否在移动
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsSlideMove()
        {
            short[] data = await modbusService.ReadHoldingRegistersAsync(41, 1);
            if (data != null && data.Length > 0)
            {
                return data[0] == 1;
            }
            return false;
        }
        /// <summary>
        /// 判断做转盘是否在运动
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsLeftMove()
        {
            short[] data = await modbusService.ReadHoldingRegistersAsync(21, 1);
            if (data != null && data.Length > 0)
            {
                return data[0] == 1;
            }
            return false;
        }
        /// <summary>
        /// 判断右转盘是否在运动
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsRightMove()
        {
            short[] data = await modbusService.ReadHoldingRegistersAsync(31, 1);
            if (data != null && data.Length > 0)
            {
                return data[0] == 1;
            }
            return false;
        }



    }
}
