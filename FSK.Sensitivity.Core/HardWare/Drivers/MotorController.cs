using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Model;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<MotorController> logger;

        public MotorController(IModbusService modbusService,ILogger<MotorController> logger)
        {
            this.modbusService = modbusService;
            this.logger = logger;
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
            logger.LogInformation($"SetSlideBlock:{position}");
            if (position < 50 || position > 80)
            {
                return true;
            }
            short pd = (short)(position - 50);
            logger.LogInformation($"pd:{pd}");
            return await modbusService.WriteSingleRegisterAsync(8, pd);

        }
        /// <summary>
        /// 判断丝杆是否在移动
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsSlideMove()
        {
            logger.LogInformation($"IsSlideMove");
            short[] data = await modbusService.ReadHoldingRegistersAsync(41, 1);
            if (data != null && data.Length > 0)
            {
                logger.LogInformation($"IsSlideMove:{data[0]}");
                
                return data[0] == 2;
            }
            return false;
        }
        /// <summary>
        /// 判断左转盘是否在运动
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsLeftMove()
        {
            logger.LogInformation($"IsLeftMove");
            short[] data = await modbusService.ReadHoldingRegistersAsync(21, 1);
            if (data != null && data.Length > 0)
            {
                logger.LogInformation($"IsLeftMove:{data[0]}");
                return data[0] == 2;
            }
            return false;
        }
        /// <summary>
        /// 判断右转盘是否在运动
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsRightMove()
        {
            logger.LogInformation($"IsRightMove");
            short[] data = await modbusService.ReadHoldingRegistersAsync(31, 1);
            
            if (data != null && data.Length > 0)
            {
                logger.LogInformation($"IsRightMove:{data[0]}");
                return data[0] == 2;
            }
            return false;
        }
        /// <summary>
        /// 停止所有电机
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StopAllMotor()
        {
            bool leftStop = await modbusService.WriteSingleRegisterAsync(21, 0);//设置左侧电机停止
            bool rightStop = await modbusService.WriteSingleRegisterAsync(31, 1);//设置右侧电机停止
            bool slideStop = await modbusService.WriteSingleRegisterAsync(41, 1);//设置丝杆电机停止
            return leftStop && rightStop && slideStop;
        }

        /// <summary>
        /// 判断电机是否全部停止
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsAllStop()
        {
            bool isLeftMove = await IsLeftMove();
            bool isRightMove = await IsRightMove();
            bool isSlideMove = await IsSlideMove();
            logger.LogInformation($"IsLeftMove:{isLeftMove},IsRightMove:{isRightMove},IsSlideMove:{isSlideMove}");
            return !isLeftMove && !isRightMove && !isSlideMove;
        }



    }
}
