using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.Infrastructure
{
    public interface IHeartbeatService
    {
        void Start(string deviceNO);
        void Stop();
        bool IsRunning { get; }
    }


    public class HeartbeatService : IHeartbeatService
    {
        private CancellationTokenSource _cancellationTokenSource;
        private Task _heartbeatTask;
        private bool _isRunning;

        // 心跳间隔（毫秒）
        private readonly int _heartbeatInterval;

        // 日志接口（可选）
        private readonly ILogger<HeartbeatService> _logger;
        private readonly ICloudSyncService cloudSyncService;
        private string deviceNO;

        public bool IsRunning
        {
            get { return _isRunning; }
            private set { _isRunning = value; }
        }

        public HeartbeatService(ILogger<HeartbeatService> logger,ICloudSyncService cloudSyncService)
        {
            _logger = logger;
            this.cloudSyncService = cloudSyncService;
            _heartbeatInterval = 5000; // 默认5秒
        }

        public void Start(string deviceNO)
        {
            this.deviceNO = deviceNO;
            if (IsRunning)
            {
                _logger?.LogWarning("心跳服务已在运行中");
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            _heartbeatTask = Task.Run(() => SendHeartbeat(_cancellationTokenSource.Token), _cancellationTokenSource.Token);
            IsRunning = true;
            _logger?.LogInformation("心跳服务已启动");
        }

        public void Stop()
        {
            if (!IsRunning)
            {
                return;
            }

            _cancellationTokenSource?.Cancel();
            try
            {
                _heartbeatTask?.Wait(TimeSpan.FromSeconds(2));
            }
            catch (OperationCanceledException)
            {
                _logger?.LogInformation("心跳任务已取消");
            }
            finally
            {
                _cancellationTokenSource?.Dispose();
                IsRunning = false;
                _logger?.LogInformation("心跳服务已停止");
            }
        }

        private async Task SendHeartbeat(CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        // 发送心跳包的实现
                        await SendHeartbeatPacketAsync(cancellationToken);
                        _logger?.LogDebug("心跳包已发送");
                    }
                    catch (Exception ex)
                    {
                        _logger?.LogError(ex, "发送心跳包时出错");
                    }

                    // 等待指定的时间间隔
                    await Task.Delay(_heartbeatInterval, cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                _logger?.LogInformation("心跳线程已取消");
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "心跳服务发生异常");
            }
        }

        private async Task SendHeartbeatPacketAsync(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(deviceNO))
            { 
                await cloudSyncService.HeartBeatAsync(deviceNO);
            
            }
        }
    }

}
