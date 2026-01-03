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
        Task<short[]> ReadHoldingRegistersAsync(ushort startAddress, ushort count);

        // 写入单个寄存器
        Task<bool> WriteSingleRegisterAsync(ushort registeraddress, short value);

        // 写入多个寄存器
        Task<bool> WriteMultipleRegistersAsync(ushort startAddress, short[] values);
    }
}
