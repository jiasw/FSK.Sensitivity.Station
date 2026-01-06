using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.HardWare;
using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Repositories;
using FSK.Sensitivity.Main.Controls;
using HandyControl.Controls;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class SerialPortConfigViewModel : BindableBase
    {

       
        private string motorSerialPortName;
        /// <summary>
        /// 硬件串口名称
        /// </summary>
        public string MotorSerialPortName
        {
            get { return motorSerialPortName; }
            set { SetProperty(ref motorSerialPortName, value); }
        }
        
        private List<string> serialPortNames;
        /// <summary>
        /// 串口名称列表
        /// </summary>
        public List<string> SerialPortNames
        {
            get { return serialPortNames; }
            set { SetProperty(ref serialPortNames, value); }
        }
        private SerialPortHelper serialPortHelper=new SerialPortHelper();
        
        private readonly IModbusService modbusService;
        private readonly IConfigurationService configurationService;

        public SerialPortConfigViewModel( IModbusService modbusService, IConfigurationService configurationService)
        {
            SerialPortNames=serialPortHelper.GetAvailableSerialPorts().ToList();
            this.modbusService = modbusService;
            this.configurationService = configurationService;
        }

        

        public DelegateCommand LoadCommand => new DelegateCommand(Load);

        private List<Dict> list;
        private void Load()
        {
            AppSetting appSetting= configurationService.LoadSetting();
            MotorSerialPortName = appSetting.ModbusPortName;
        }

        public DelegateCommand SaveCommand => new DelegateCommand( Save);
        private void Save()
        {
            AppSetting appSetting = configurationService.LoadSetting();
            appSetting.ModbusPortName=MotorSerialPortName;
            configurationService.SaveSetting(appSetting);
            modbusService.Initialize(MotorSerialPortName);
            MessageBoxService.Instance.Show("保存成功");
        }
        
        
    }
}
