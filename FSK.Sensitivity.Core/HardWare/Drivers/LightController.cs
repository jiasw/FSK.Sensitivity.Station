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
        System.IO.Ports.SerialPort serialPort;
        public LightController(SerialPortConfig config)
        {
            Config = config;
            Initalize();
        }


        private void Initalize()
        {
            if (Config == null|| string.IsNullOrEmpty(Config.SerialPortName))
            {
                LogHelper.Instance.LogError("灯光控制器配置为空");
            }
            try
            {
                SerialPortHelper serialPortHelper = new SerialPortHelper();
               bool isOpen =serialPortHelper.OpenSerialPort(Config.SerialPortName, Config.BaudRate, Config.DataBits, Config.Parity, Config.StopBits);
                if (isOpen)
                {
                    serialPort = serialPortHelper.GetSerialPort();
                    _isAvailable = true;
                }
                else
                {
                    LogHelper.Instance.LogError("打开串口失败");
                }
            }
            catch (Exception ex)
            { 
                LogHelper.Instance.LogError( "打开串口失败", ex);
            }

        }

        private bool _isAvailable = false;
        public bool IsAvailable => _isAvailable;

        public SerialPortConfig Config { get; }

        public void TurnOff()
        {
            
        }

        public void TurnOn()
        {
           
        }

        public void WeakLight()
        {
            
        }
    }
}
