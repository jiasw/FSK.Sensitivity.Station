using FSK.Sensitivity.Core.HardWare.Peripherals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Drivers
{
    public class SpeechService : ISpeechService
    {
        private SpeechSynthesizer _synthesizer;
        private bool _disposed = false;
        public SpeechService()
        {
            _synthesizer = new SpeechSynthesizer();
            _synthesizer.Volume = 100;
            _synthesizer.Rate = 0;
        }
        public bool IsSpeaking => _synthesizer.State == SynthesizerState.Speaking;
        public int Volume
        {
            get => _synthesizer.Volume;
            private set => _synthesizer.Volume = value;
        }
        public int Rate
        {
            get => _synthesizer.Rate;
            private set => _synthesizer.Rate = value;
        }
        public string CurrentVoiceName => _synthesizer.Voice.Name;
        public void Speak(string text)
        {
            CheckDisposed();
            _synthesizer.Speak(text);
        }
        public async Task SpeakAsync(string text)
        {
            CheckDisposed();
            await Task.Run(() => _synthesizer.Speak(text));
        }
        public void SpeakAsyncImmediate(string text)
        {
            CheckDisposed();
            _synthesizer.SpeakAsync(text);
        }
        public void Stop()
        {
            CheckDisposed();
            _synthesizer.SpeakAsyncCancelAll();
        }
        public IEnumerable<InstalledVoice> GetInstalledVoices()
        {
            CheckDisposed();
            return _synthesizer.GetInstalledVoices();
        }
        public void SetVoice(string voiceName)
        {
            CheckDisposed();
            _synthesizer.SelectVoice(voiceName);
        }
        public void SetVolume(int volume)
        {
            CheckDisposed();
            Volume = Math.Max(0, Math.Min(100, volume));
        }
        public void SetRate(int rate)
        {
            CheckDisposed();
            Rate = Math.Max(-10, Math.Min(10, rate));
        }
        public void Pause()
        {
            CheckDisposed();
            _synthesizer.Pause();
        }
        public void Resume()
        {
            CheckDisposed();
            _synthesizer.Resume();
        }
        public void SaveToFile(string text, string fileName, SynthesisMediaType format = SynthesisMediaType.WaveAudio)
        {
            CheckDisposed();
            _synthesizer.SetOutputToWaveFile(fileName);
            _synthesizer.Speak(text);
            _synthesizer.SetOutputToDefaultAudioDevice();
        }
        
        public void AddSpeakCompletedEventHandler(EventHandler<SpeakCompletedEventArgs> handler)
        {
            CheckDisposed();
            _synthesizer.SpeakCompleted += handler;
        }
        public void RemoveSpeakCompletedEventHandler(EventHandler<SpeakCompletedEventArgs> handler)
        {
            CheckDisposed();
            _synthesizer.SpeakCompleted -= handler;
        }
        private void CheckDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(SpeechService));
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _synthesizer?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
