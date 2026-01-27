using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Peripherals
{
    /// <summary>
    /// 语音服务接口
    /// </summary>
    public interface ISpeechService : IDisposable
    {
        /// <summary>
        /// 同步播放文本语音
        /// </summary>
        /// <param name="text">要播放的文本</param>
        void Speak(string text);
        /// <summary>
        /// 异步播放文本语音
        /// </summary>
        /// <param name="text">要播放的文本</param>
        /// <returns>任务</returns>
        Task SpeakAsync(string text);
        /// <summary>
        /// 立即异步播放文本语音（不等待完成）
        /// </summary>
        /// <param name="text">要播放的文本</param>
        void SpeakAsyncImmediate(string text);
        /// <summary>
        /// 停止所有正在播放的语音
        /// </summary>
        void Stop();
        /// <summary>
        /// 获取所有已安装的语音
        /// </summary>
        /// <returns>语音列表</returns>
        IEnumerable<InstalledVoice> GetInstalledVoices();
        /// <summary>
        /// 根据语音名称设置当前语音
        /// </summary>
        /// <param name="voiceName">语音名称</param>
        void SetVoice(string voiceName);
        /// <summary>
        /// 设置语音音量
        /// </summary>
        /// <param name="volume">音量值 (0-100)</param>
        void SetVolume(int volume);
        /// <summary>
        /// 设置语音语速
        /// </summary>
        /// <param name="rate">语速值 (-10 到 10)</param>
        void SetRate(int rate);
        /// <summary>
        /// 暂停语音播放
        /// </summary>
        void Pause();
        /// <summary>
        /// 恢复语音播放
        /// </summary>
        void Resume();
        /// <summary>
        /// 获取当前是否正在播放语音
        /// </summary>
        bool IsSpeaking { get; }
        /// <summary>
        /// 获取当前语音音量
        /// </summary>
        int Volume { get; }
        /// <summary>
        /// 获取当前语音语速
        /// </summary>
        int Rate { get; }
        /// <summary>
        /// 获取当前使用的语音名称
        /// </summary>
        string CurrentVoiceName { get; }
        /// <summary>
        /// 保存语音到文件
        /// </summary>
        /// <param name="text">要合成的文本</param>
        /// <param name="fileName">保存的文件名</param>
        /// <param name="format">音频格式</param>
        void SaveToFile(string text, string fileName, SynthesisMediaType format = SynthesisMediaType.WaveAudio);
       
        /// <summary>
        /// 添加语音播放完成事件监听
        /// </summary>
        /// <param name="handler">事件处理函数</param>
        void AddSpeakCompletedEventHandler(EventHandler<SpeakCompletedEventArgs> handler);
        /// <summary>
        /// 移除语音播放完成事件监听
        /// </summary>
        /// <param name="handler">事件处理函数</param>
        void RemoveSpeakCompletedEventHandler(EventHandler<SpeakCompletedEventArgs> handler);
    }
}
