using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Entity;
using FSK.Sensitivity.Core.HardWare;
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

        private string handlerSerialPortName;
        /// <summary>
        /// 手柄串口名称
        /// </summary>
        public string HandlerSerialPortName
        {
            get { return handlerSerialPortName; }
            set { SetProperty(ref handlerSerialPortName, value); }
        }
        private string motorSerialPortName;
        /// <summary>
        /// 电机串口名称
        /// </summary>
        public string MotorSerialPortName
        {
            get { return motorSerialPortName; }
            set { SetProperty(ref motorSerialPortName, value); }
        }
        private string lightSerialPortName;
        /// <summary>
        /// 光敏串口名称
        /// </summary>
        public string LightSerialPortName
        {
            get { return lightSerialPortName; }
            set { SetProperty(ref lightSerialPortName, value); }
        }
        private string scannerSerialPortName;
        /// <summary>
        /// 扫描仪串口名称
        /// </summary>
        public string ScannerSerialPortName
        {
            get { return scannerSerialPortName; }   
            set { SetProperty(ref scannerSerialPortName, value); }
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
        private readonly DictRepository dictRepository;

        public SerialPortConfigViewModel( DictRepository dictRepository)
        {
            SerialPortNames=serialPortHelper.GetAvailableSerialPorts().ToList();
            this.dictRepository = dictRepository;
        }

        

        public DelegateCommand LoadCommand => new DelegateCommand(async () => await Load());

        private List<Dict> list;
        private async Task Load()
        {
            list = await dictRepository.Query(n => n.TypeCode == AppConst.Dict_TypeCode_SerialPort);
            foreach (var item in list)
            {
                if (item.Code == AppConst.Dict_Key_HandlePort)
                {
                    HandlerSerialPortName = item.Name;
                }
                else if (item.Code == AppConst.Dict_Key_MotorPort)
                {
                    MotorSerialPortName = item.Name;
                }
                else if (item.Code == AppConst.Dict_Key_LightPort)
                {
                    LightSerialPortName = item.Name;
                }
                else if (item.Code == AppConst.Dict_Key_ScannerPort)
                {
                    ScannerSerialPortName = item.Name;
                }
            }
        }

        public DelegateCommand SaveCommand => new DelegateCommand(async () => await Save());
        private async Task Save()
        {
            AppData.Instance.HardWareConfigIsChanged = true;
            if (list.Exists(n => n.Code == AppConst.Dict_Key_HandlePort))
            {
                var model=list.Where(n => n.Code == AppConst.Dict_Key_HandlePort).First();
                //更新
                await dictRepository.Update(model);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(HandlerSerialPortName))
                {
                    await dictRepository.Add(new Dict()
                    {
                        TypeCode = AppConst.Dict_TypeCode_SerialPort,
                        Code = AppConst.Dict_Key_HandlePort,
                        Name = HandlerSerialPortName,
                        Description = "手柄串口"
                    });
                }
                
            }
            if (list.Exists(n => n.Code == AppConst.Dict_Key_MotorPort))
            {
                var model = list.Where(n => n.Code == AppConst.Dict_Key_MotorPort).First();
                //更新
                await dictRepository.Update(model);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(MotorSerialPortName))
                {
                    await dictRepository.Add(new Dict()
                    {
                        TypeCode = AppConst.Dict_TypeCode_SerialPort,
                        Code = AppConst.Dict_Key_MotorPort,
                        Name = MotorSerialPortName,
                        Description = "电机串口"
                    });
                }
                
            }
            if (list.Exists(n => n.Code == AppConst.Dict_Key_LightPort))
            {
                var model = list.Where(n => n.Code == AppConst.Dict_Key_LightPort).First();
                //更新
                await dictRepository.Update(model);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace( LightSerialPortName) )
                {
                    await dictRepository.Add(new Dict()
                    {
                        TypeCode = AppConst.Dict_TypeCode_SerialPort,
                        Code = AppConst.Dict_Key_LightPort,
                        Name = LightSerialPortName,
                        Description = "光敏串口"
                    });
                }
                
            }
            if (list.Exists(n => n.Code == AppConst.Dict_Key_ScannerPort))
            {
                var model = list.Where(n => n.Code == AppConst.Dict_Key_ScannerPort).First();
                //更新
                await dictRepository.Update(model);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(ScannerSerialPortName))
                {
                    await dictRepository.Add(new Dict()
                    {
                        TypeCode = AppConst.Dict_TypeCode_SerialPort,
                        Code = AppConst.Dict_Key_ScannerPort,
                        Name = ScannerSerialPortName,
                        Description = "扫描仪串口"
                    });
                }
                
            }
            AlertMessageBox.Show("保存成功");
        }
        
        
    }
}
