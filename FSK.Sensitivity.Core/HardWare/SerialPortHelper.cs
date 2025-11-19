using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare
{
    public class SerialPortHelper
    {
        public string[] GetAvailableSerialPorts()
        {
            return System.IO.Ports.SerialPort.GetPortNames();
        }

        public bool IsSerialPortAvailable(string portName)
        {
            return System.IO.Ports.SerialPort.GetPortNames().Contains(portName);
        }


        System.IO.Ports.SerialPort serialPort;

       
        public bool OpenSerialPort(string portName, int baudRate, int dataBits, System.IO.Ports.Parity parity, System.IO.Ports.StopBits stopBits)
        {
            try
            {
                serialPort = new System.IO.Ports.SerialPort(portName, baudRate, parity, dataBits, stopBits);
                serialPort.Open();  
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void CloseSerialPort()
        {
            if (serialPort!= null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
        
        public System.IO.Ports.SerialPort GetSerialPort()
        {
            return serialPort;
        }

    }
}
