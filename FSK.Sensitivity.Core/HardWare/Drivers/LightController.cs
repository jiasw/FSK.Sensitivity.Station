
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Drivers
{
    /// <summary>
    /// 灯光控制器
    /// </summary>
    public class LightController : ILight
    {
        private readonly IModbusService modbusService;

        public LightController(IModbusService modbusService)
        {
            this.modbusService = modbusService;
        }

        public async Task<bool> TurnOnLeft()
        {
           return await modbusService.WriteSingleRegisterAsync( 4, 1);
        }

        public async Task<bool> TurnOnRight()
        {
            return await modbusService.WriteSingleRegisterAsync(5, 1);
        }

        public async Task<bool> TurnOffLeft()
        {
            return await modbusService.WriteSingleRegisterAsync(4, 0);
        }

        public async Task<bool> TurnOffRight()
        {
            return await modbusService.WriteSingleRegisterAsync(4, 0);
        }

        public async Task<bool> TurnOnAll()
        {
            return await modbusService.WriteMultipleRegistersAsync(4, new short[] { 1, 1 });
            
        }

        public async Task<bool> TurnOffAll()
        {
            return await modbusService.WriteMultipleRegistersAsync(4, new short[] { 0, 0 });
        }

        

        
    }
}
