using FluentModbus;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Text;

namespace FSK.Sensitivity.Core.HardWare.Drivers
{
    public class ModbusService : IModbusService, IDisposable
    {
        // 锁对象，确保 RTU 这种半双工通讯在并发调用时不会冲突
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly ILogger<ModbusService> logger;
        private ModbusRtuClient _client;
        
        private bool _isConnected;
        public bool IsConnected
        {
            get=> _isConnected;
            set => _isConnected = value;
        }
        private string _portName;
        private byte _deviceId = 1; // 默认设备ID，可根据需要修改

        public ModbusService(ILogger<ModbusService> logger)
        {
            this.logger = logger;
        }

        public void Initialize(string portName)
        {
            _portName = portName;
            _client = new ModbusRtuClient()
            {
                BaudRate=115200,
                Parity=Parity.None,
                ReadTimeout=1000,
                WriteTimeout=1000,
               
            };
           

            _isConnected = false;
        }

        

        public bool Connect()
        {
            try
            {
                if (!IsConnected)
                {
                    // 参数：端口号, 波特率, 校验位, 数据位, 停止位
                    _client.Connect(_portName, ModbusEndianness.BigEndian);
                    _isConnected = true;
                }
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"串口打开失败", ex);
                return false;
            }
        }

        private bool EnsureConnection()
        {
            if (!IsConnected) return Connect();
            return true;
        }
        // 读取保持寄存器
        // 替换 ReadHoldingRegistersAsync 方法，避免在异步方法中使用 Span/unsafe
        public async Task<short[]> ReadHoldingRegistersAsync(ushort startAddress, ushort count)
        {
            
            await _lock.WaitAsync(); // RTU 必须等待上一个指令完成
            try
            {
                if (!EnsureConnection()) return null;

                var data = _client.ReadHoldingRegisters<short>(_deviceId, startAddress, count);
                short[] result = new short[data.Length];
                data.CopyTo(result);
                return result;
            }
            catch (Exception ex)
            {
                logger.LogError($"串口读取失败", ex);
                return null;
            }
            finally
            {
                _lock.Release();
            }
        }

        
        // 写入单个寄存器
        public async Task<bool> WriteSingleRegisterAsync(ushort registeraddress, short value)
        {
            await _lock.WaitAsync();
            try
            {
                if (!EnsureConnection()) return false;
                //ushort finalValue =  (ushort)((value << 8) | (value >> 8)) ;
                _client.WriteSingleRegister(_deviceId, registeraddress, value);
                return true;
                
            }
            catch (Exception ex)
            {
                logger.LogError($"串口写入失败",ex);
                return false;
            }
            finally
            {
                
                _lock.Release();
            }
        }

        // 写入多个寄存器
        public async Task<bool> WriteMultipleRegistersAsync(ushort startAddress, short[] values)
        {
            await _lock.WaitAsync();
            try
            {
                if (!EnsureConnection()) return false;
                _client.WriteMultipleRegisters(_deviceId, startAddress, values);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError($"串口写入失败", ex);
                return false;
            }
            finally
            {
                _lock.Release();
            }
        }

        public void Dispose()
        {
            _client?.Dispose();
            _lock?.Dispose();
        }
    }
}
