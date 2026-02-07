using FSK.Sensitivity.Core.Model;
using System;
using System.Collections.Generic;

namespace FSK.Sensitivity.Core.Infrastructure
{
    public enum TrainingStatus
    {
        None = 0,
        Initializing,
        Started,
        Completed,
        Cancelled
    }

    // 训练项目事件参数
    public class TrainingItemEventArgs : EventArgs
    {
        public ItemOrder Item { get; }
        public int ItemIndex { get; }
        public int TotalItems { get; }

        public TrainingItemEventArgs(ItemOrder item, int itemIndex, int totalItems)
        {
            Item = item;
            ItemIndex = itemIndex;
            TotalItems = totalItems;
        }
    }

    public interface ITrainingAndCheckService
    {
        event EventHandler<TrainingItemEventArgs> TrainingItemStarted;
        event EventHandler<TrainingItemEventArgs> TrainingItemCompleted;
        event EventHandler TrainingFinished;

        void Initialize(List<ItemOrder> items);
        void StartTraining();                   // 开始训练
        ItemOrder GetCurrentItem();            // 获取当前训练项目
        void CompleteCurrentItem();            // 完成当前训练项
        void FinishTraining();                 // 主动结束整个训练流程
        void Reset();                          // 重置训练状态，清空数据

        TrainingStatus CurrentStatus { get; }
        int CurrentItemIndex { get; }
        int TotalItems { get; }
    }

    public class TrainingAndCheckService : ITrainingAndCheckService
    {
        private TrainingStatus _currentStatus = TrainingStatus.None;
        private List<ItemOrder> _trainingItems = new();
        private int _currentItemIndex = -1;

        public event EventHandler<TrainingItemEventArgs> TrainingItemStarted;
        public event EventHandler<TrainingItemEventArgs> TrainingItemCompleted;
        public event EventHandler TrainingFinished;

        public TrainingStatus CurrentStatus => _currentStatus;
        public int CurrentItemIndex => _currentItemIndex;
        public int TotalItems => _trainingItems?.Count ?? 0;

        public void Initialize(List<ItemOrder> items)
        {
            if (_currentStatus != TrainingStatus.None && _currentStatus != TrainingStatus.Completed)
                throw new InvalidOperationException("只能在未开始或已完成状态下初始化");

            _trainingItems = items ?? new List<ItemOrder>();
            _currentItemIndex = -1;
            _currentStatus = TrainingStatus.Initializing;
        }

        public void StartTraining()
        {
            if (_currentStatus != TrainingStatus.Initializing)
                throw new InvalidOperationException("必须先初始化");

            _currentStatus = TrainingStatus.Started;
            StartNextItem();
        }

        public ItemOrder GetCurrentItem()
        {
            if (_currentStatus != TrainingStatus.Started)
                return null;

            if (_currentItemIndex >= 0 && _currentItemIndex < _trainingItems.Count)
                return _trainingItems[_currentItemIndex];

            return null;
        }

        public void CompleteCurrentItem()
        {
            if (_currentStatus != TrainingStatus.Started)
                return;

            if (_currentItemIndex >= 0 && _currentItemIndex < _trainingItems.Count)
            {
                var currentItem = _trainingItems[_currentItemIndex];
                OnTrainingItemCompleted(new TrainingItemEventArgs(currentItem, _currentItemIndex, TotalItems));

                if (_currentItemIndex == _trainingItems.Count - 1)
                {
                    FinishTraining();
                }
                else
                {
                    StartNextItem();
                }
            }
        }

        public void FinishTraining()
        {
            if (_currentStatus == TrainingStatus.Started || _currentStatus == TrainingStatus.Initializing)
            {
                _currentStatus = TrainingStatus.Completed;
                OnTrainingFinished();
            }
        }

        public void Reset()
        {
            _currentStatus = TrainingStatus.None;
            _trainingItems.Clear();
            _currentItemIndex = -1;
        }

        private void StartNextItem()
        {
            if (_trainingItems == null || ++_currentItemIndex >= _trainingItems.Count)
                return;

            var nextItem = _trainingItems[_currentItemIndex];
            OnTrainingItemStarted(new TrainingItemEventArgs(nextItem, _currentItemIndex, TotalItems));
        }

        protected virtual void OnTrainingItemStarted(TrainingItemEventArgs e) =>
            TrainingItemStarted?.Invoke(this, e);

        protected virtual void OnTrainingItemCompleted(TrainingItemEventArgs e) =>
            TrainingItemCompleted?.Invoke(this, e);

        protected virtual void OnTrainingFinished() =>
            TrainingFinished?.Invoke(this, EventArgs.Empty);
    }
}
