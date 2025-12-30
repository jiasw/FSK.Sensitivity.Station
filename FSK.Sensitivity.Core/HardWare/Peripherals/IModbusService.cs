using System;
using System.Collections.Generic;
using System.Text;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    public interface IModbusService
    {

        
        bool IsConnected { get; }
        // 允许传入新的配置参数
        void Initialize(string portName);
        bool Connect();
        // 读取保持寄存器
        // 替换 ReadHoldingRegistersAsync 方法，避免在异步方法中使用 Span/unsafe
        Task<short[]> ReadHoldingRegistersAsync(int startAddress, int count);

        // 写入单个寄存器
        Task<bool> WriteSingleRegisterAsync(int registeraddress, short value);
    }
}
