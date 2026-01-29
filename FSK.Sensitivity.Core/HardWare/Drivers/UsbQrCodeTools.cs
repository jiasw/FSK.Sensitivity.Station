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
    /// 扫码器输入服务 - 基于时间间隔判断扫码结束
    /// </summary>
    public interface IBarcodeScannerService
    {
        event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;
        void StartListening();
        void StopListening();
        bool IsListening { get; }
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
        // 扫码缓冲区
        private string _currentInput = "";

        // 扫码结束判断配置
        private readonly int _scanEndTimeoutMs = 100;  // 100ms无输入则判定为扫码结束
        private readonly int _antiRepeatIntervalMs = 500; // 防重复时间间隔

        // 防重复记录
        private string _lastBarcode = "";
        private DateTime _lastScanTime = DateTime.MinValue;
        // 扫码结束计时器
        private System.Timers.Timer _scanEndTimer;
        public event EventHandler<BarcodeScannedEventArgs> BarcodeScanned;
        public bool IsListening => _isListening;
        public BarcodeScannerService()
        {
            _proc = HookCallback;

            // 初始化扫码结束检测计时器
            _scanEndTimer = new System.Timers.Timer(_scanEndTimeoutMs);
            _scanEndTimer.Elapsed += ScanEndTimer_Elapsed;
            _scanEndTimer.AutoReset = false; // 只触发一次，需要手动重启
        }
        public void StartListening()
        {
            if (_isListening)
                return;
            _hookHandle = SetHook(_proc);
            _isListening = true;
            _currentInput = "";
            _lastBarcode = "";
            Debug.WriteLine("扫码器开始监听");
        }
        public void StopListening()
        {
            if (!_isListening)
                return;
            UnhookWindowsHookEx(_hookHandle);
            _isListening = false;
            _currentInput = "";
            _scanEndTimer.Stop();
            Debug.WriteLine("扫码器停止监听");
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

                    Debug.WriteLine($"扫码输入: {keyChar}, 当前缓冲: {_currentInput}");
                    // 重置扫码结束计时器
                    _scanEndTimer.Stop();
                    _scanEndTimer.Start();
                    // 拦截键盘输入，防止扫码内容传递给应用
                    return (IntPtr)1;
                }
            }
            return CallNextHookEx(_hookHandle, nCode, wParam, lParam);
        }
        /// <summary>
        /// 扫码结束计时器事件处理
        /// </summary>
        private void ScanEndTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_currentInput.Length > 0)
            {
                ProcessBarcode(_currentInput.Trim());
                _currentInput = "";
            }
        }
        /// <summary>
        /// 处理扫码数据
        /// </summary>
        private void ProcessBarcode(string barcode)
        {
            // 防重复检查
            if (barcode == _lastBarcode &&
                DateTime.Now.Subtract(_lastScanTime).TotalMilliseconds < _antiRepeatIntervalMs)
            {
                Debug.WriteLine($"扫码重复检测：忽略 {barcode}");
                return;
            }
            _lastBarcode = barcode;
            _lastScanTime = DateTime.Now;
            Debug.WriteLine($"扫码完成: {barcode}");
            BarcodeScanned?.Invoke(this, new BarcodeScannedEventArgs
            {
                Barcode = barcode,
                ScanTime = DateTime.Now
            });
        }
        /// <summary>
        /// 尝试从虚拟键码获取特殊字符
        /// </summary>
        private bool TryGetCharFromVKey(int vkCode, out char result)
        {
            result = '\0';
            var symbolMap = new Dictionary<int, char>
            {
                { 189, '-' },   // 减号
                { 187, '=' },   // 等号
                { 219, '[' },   // 左方括号
                { 221, ']' },   // 右方括号
                { 186, ';' },   // 分号
                { 222, '\'' },  // 单引号
                { 188, ',' },   // 逗号
                { 190, '.' },   // 句号
                { 191, '/' },   // 斜杠
                { 220, '\\' },  // 反斜杠
                { 192, '`' },   // 反引号
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
