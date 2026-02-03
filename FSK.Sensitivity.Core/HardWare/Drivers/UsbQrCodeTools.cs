using FSK.Sensitivity.Core.HardWare.Peripherals;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Interop;

namespace FSK.Sensitivity.Core.HardWare.Drivers
{
    


    /// <summary>
    /// 扫码器输入服务 - 改进版本
    /// 1. 只在 StartListening 后才开始处理扫码
    /// 2. StartListening 后有初始化延迟，防止误触
    /// 3. 防重复和防抖动机制
    /// </summary>
    public interface IBarcodeScannerService
    {
        event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;
        void StartListening();
        void StopListening();
        bool IsListening { get; }
        bool IsEnabled { get; }  // 区分"监听中"和"已启用"
    }

    public class BarcodeScannedEventArgs : EventArgs
    {
        public string Barcode { get; set; }
        public DateTime ScanTime { get; set; }
    }

    public class BarcodeScannerService : IBarcodeScannerService
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private IntPtr _hookHandle = IntPtr.Zero;
        private LowLevelKeyboardProc _proc;
        private bool _isListening = false;

        /// <summary>
        /// 关键：标记扫码器是否真正启用（解决防误触问题）
        /// </summary>
        private bool _isEnabled = false;

        // 扫码缓冲区
        private string _currentInput = "";

        // 扫码结束判断配置
        private readonly int _scanEndTimeoutMs = 100;          // 100ms无输入则判定为扫码结束
        private readonly int _antiRepeatIntervalMs = 500;      // 防重复时间间隔
        private readonly int _initializationDelayMs = 300;     // 启动后延迟300ms才启用，防误触

        // 防重复记录
        private string _lastBarcode = "";
        private DateTime _lastScanTime = DateTime.MinValue;

        // 扫码结束计时器
        private System.Timers.Timer _scanEndTimer;

        // 启用延迟计时器（防误触）
        private System.Timers.Timer _enableDelayTimer;

        public event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;
        public bool IsListening => _isListening;
        public bool IsEnabled => _isEnabled;

        public BarcodeScannerService()
        {
            _proc = HookCallback;

            // 初始化扫码结束检测计时器
            _scanEndTimer = new System.Timers.Timer(_scanEndTimeoutMs);
            _scanEndTimer.Elapsed += ScanEndTimer_Elapsed;
            _scanEndTimer.AutoReset = false;

            // 初始化启用延迟计时器
            _enableDelayTimer = new System.Timers.Timer(_initializationDelayMs);
            _enableDelayTimer.Elapsed += EnableDelayTimer_Elapsed;
            _enableDelayTimer.AutoReset = false;
        }

        public void StartListening()
        {
            if (_isListening)
                return;

            // 1. 设置钩子
            _hookHandle = SetHook(_proc);
            _isListening = true;

            // 2. 初始化状态
            _currentInput = "";
            _lastBarcode = "";
            _isEnabled = false;  // 关键：先禁用

            // 3. 启动延迟计时器，防止立即处理扫码（防误触）
            _enableDelayTimer.Stop();
            _enableDelayTimer.Start();

            Debug.WriteLine("扫码器开始监听（初始化中...）");
            Log.Information("扫码器开始监听，{DelayMs}ms后启用", _initializationDelayMs);
        }

        public void StopListening()
        {
            if (!_isListening)
                return;

            // 1. 清理钩子
            UnhookWindowsHookEx(_hookHandle);
            _isListening = false;

            // 2. 禁用扫码处理
            _isEnabled = false;

            // 3. 清理定时器
            _scanEndTimer.Stop();
            _enableDelayTimer.Stop();

            // 4. 清理缓冲区
            _currentInput = "";

            Debug.WriteLine("扫码器停止监听");
            Log.Information("扫码器停止监听");
        }

        /// <summary>
        /// 延迟启用计时器事件 - 防止误触
        /// </summary>
        private void EnableDelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _isEnabled = true;
            Debug.WriteLine("扫码器已启用");
            Log.Information("扫码器已启用，可以开始扫码");
        }

        private IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
                return CallNextHookEx(_hookHandle, nCode, wParam, lParam);

            // 关键检查：只有启用才处理
            if (!_isEnabled)
            {
                return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
            }

            if (wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                char keyChar = '\0';
                bool isValidInput = false;

                // 数字 0-9
                if (vkCode >= 48 && vkCode <= 57)
                {
                    keyChar = (char)vkCode;
                    isValidInput = true;
                }
                // 字母 A-Z
                else if (vkCode >= 65 && vkCode <= 90)
                {
                    bool isShift = (GetKeyState(160) & 0x8000) != 0;
                    bool isCapsLock = (GetKeyState(20) & 1) != 0;
                    keyChar = (char)vkCode;
                    if (!isShift && !isCapsLock)
                        keyChar = char.ToLower(keyChar);
                    isValidInput = true;
                }
                // 特殊字符
                else if (vkCode == 13)  // Enter键 - 扫码结束
                {
                    isValidInput = false;
                    if (_currentInput.Length > 0)
                    {
                        _scanEndTimer.Stop();
                        ProcessBarcode(_currentInput.Trim());
                        _currentInput = "";
                    }
                    return (IntPtr)1;  // 拦截Enter
                }
                else
                {
                    if (TryGetCharFromVKey(vkCode, out char specialChar))
                    {
                        keyChar = specialChar;
                        isValidInput = true;
                    }
                }

                if (isValidInput)
                {
                    // 追加字符到缓冲区
                    _currentInput += keyChar;

                    Debug.WriteLine($"扫码输入: {keyChar}, 缓冲: {_currentInput}, 启用状态: {_isEnabled}");

                    // 重置扫码结束计时器
                    _scanEndTimer.Stop();
                    _scanEndTimer.Start();

                    // 拦截键盘输入
                    return (IntPtr)1;
                }
            }

            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }

        private void ScanEndTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_currentInput.Length > 0 && _isEnabled)
            {
                ProcessBarcode(_currentInput.Trim());
                _currentInput = "";
            }
        }

        private void ProcessBarcode(string barcode)
        {
            // 防重复检查
            if (barcode == _lastBarcode &&
                DateTime.Now.Subtract(_lastScanTime).TotalMilliseconds < _antiRepeatIntervalMs)
            {
                Debug.WriteLine($"扫码重复检测：忽略 {barcode}");
                Log.Warning("扫码重复检测：忽略 {Barcode}", barcode);
                return;
            }

            _lastBarcode = barcode;
            _lastScanTime = DateTime.Now;

            Debug.WriteLine($"扫码完成: {barcode}");
            Log.Information("扫码完成: {Barcode}", barcode);

            BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs
            {
                Barcode = barcode,
                ScanTime = DateTime.Now
            });
        }

        private bool TryGetCharFromVKey(int vkCode, out char result)
        {
            result = '\0';
            var symbolMap = new Dictionary<int, char>
            {
                { 189, '-' },
                { 187, '=' },
                { 219, '[' },
                { 221, ']' },
                { 186, ';' },
                { 222, '\'' },
                { 188, ',' },
                { 190, '.' },
                { 191, '/' },
                { 220, '\\' },
                { 192, '`' },
            };
            return symbolMap.TryGetValue(vkCode, out result);
        }

        #region Win32 API
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn,
            IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        [DllImport("user32.dll")]
        private static extern short GetKeyState(int nVirtKey);
        #endregion
    }
}
