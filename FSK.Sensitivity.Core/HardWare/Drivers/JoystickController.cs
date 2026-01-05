using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Drivers
{
    public class JoystickController : IJoystick, IDisposable
    {
        private readonly IModbusService _modbusService;
        private readonly Dictionary<JoystickStatus, bool> _lastState = new();

        // 控制监听任务的核心
        private CancellationTokenSource _cts;
        private Task _monitorTask;
        private readonly object _lock = new object();

        public event EventHandler<JoystickEventArgs> Pressed;

        public JoystickController(IModbusService modbusService)
        {
            _modbusService = modbusService;
            foreach (JoystickStatus status in Enum.GetValues(typeof(JoystickStatus)))
                _lastState[status] = false;
        }

        /// <summary>
        /// 开始监听
        /// </summary>
        public void StartMonitoring()
        {
            lock (_lock)
            {
                if (_monitorTask != null && !_monitorTask.IsCompleted)
                    return; // 防止重复启动

                _cts = new CancellationTokenSource();
                _monitorTask = Task.Run(() => MonitorLoopAsync(_cts.Token), _cts.Token);
            }
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void StopMonitoring()
        {
            lock (_lock)
            {
                _cts?.Cancel();
                _cts?.Dispose();
                _cts = null;
            }
        }

        private async Task MonitorLoopAsync(CancellationToken token)
        {
            // 建议配置为可配置项
            ushort startAddress = 0x10;
            ushort numberOfPoints = 6;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    short[] registers = await _modbusService.ReadHoldingRegistersAsync(startAddress, numberOfPoints);
                    if (registers != null)
                    {
                        LogHelper.Instance.LogDebug($"摇杆状态返回: {string.Join(",", registers)}");
                        //判断数组中是否有1
                        if (registers.Any(r => r == 1))
                        {
                            string logstr = $@"摇杆按下=======================
                                                    摇杆状态返回: {string.Join(",", registers)}
                                                摇杆按下=======================";
                            LogHelper.Instance.LogDebug(logstr);
                        }
                        if (registers != null && registers.Length >= numberOfPoints)
                        {
                            ProcessButtonState(JoystickStatus.Front, registers[0]);
                            ProcessButtonState(JoystickStatus.Back, registers[1]);
                            ProcessButtonState(JoystickStatus.Left, registers[2]);
                            ProcessButtonState(JoystickStatus.Right, registers[3]);
                            ProcessButtonState(JoystickStatus.Confirm, registers[4]);
                            ProcessButtonState(JoystickStatus.Trigger, registers[5]);
                        }
                    }
                    else
                    {
                        LogHelper.Instance.LogDebug("读取摇杆状态失败,registers为null");
                    }
                    
                }
                catch (OperationCanceledException) { /* 正常退出 */ }
                catch (Exception ex)
                {
                    LogHelper.Instance.LogError("读取摇杆状态出错", ex);
                    // 这里建议增加日志记录
                    Debug.WriteLine($"Joystick Error: {ex.Message}");
                }
                await Task.Delay(5, token);
            }
        }

        private void ProcessButtonState(JoystickStatus status, short value)
        {
            bool isPressed = (value == 0x01);
            if (isPressed && !_lastState[status])
            {
                // 异步触发事件，防止订阅者阻塞监听主循环
                Task.Run(() => Pressed?.Invoke(this, new JoystickEventArgs(status)));
            }
            _lastState[status] = isPressed;
        }

        // 彻底防止内存泄漏：销毁模式
        public void Dispose()
        {
            StopMonitoring();
            // 清理事件引用，帮助垃圾回收
            Pressed = null;
            GC.SuppressFinalize(this);
        }
    }
}
