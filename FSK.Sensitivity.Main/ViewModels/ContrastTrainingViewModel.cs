using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.Utility;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace FSK.Sensitivity.Main.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ContrastTrainingViewModel : BaseViewModel, INavigationAware
    {
        private readonly SecondaryChangeEvent secondaryChangeEvent;//副屏页面切换事件
        private readonly ContrastSignChangeEvent contrastSignChangeEvent;//对比度标志位事件
        
        private string black = "black";
        private string white = "white";

        //操作界面视标图片
        private object _backgroundSource;
        //操作界面视标图片
        public object BackgroundSource
        {
            get => _backgroundSource;
            set => SetProperty(ref _backgroundSource, value);
        }
        //检查时长（秒）
        private int _checkDuration = 5;
        public int CheckDuration
        {
            get { return _checkDuration; }
            set { SetProperty(ref _checkDuration, value); }
        }

        //等待时长（秒）
        private int _waitDuration = 30;
        public int WaitDuration
        {
            get { return _waitDuration; }
            set { SetProperty(ref _waitDuration, value); }
        }

        //视标图片名称
        private string _signPictureName;
        

        
        /*
         页面每次打开前，先设置白屏等待30秒，再根据实际情况设置检查时长
         */
        private DispatcherTimer _waittimer;
        private DispatcherTimer _checktimer;

        private int imageIndex = 0;

        private ArrowButtonStauts _arrowButtonStauts = new ArrowButtonStauts();

        public ContrastTrainingViewModel(IRegionManager regionManager, IEventAggregator eventAggregator)
        {
            secondaryChangeEvent = eventAggregator.GetEvent<SecondaryChangeEvent>();
            contrastSignChangeEvent = eventAggregator.GetEvent<ContrastSignChangeEvent>();
            
            _waittimer = new DispatcherTimer();
            _waittimer.Interval = TimeSpan.FromSeconds(1);
            _waittimer.Tick += Timer_Tick;
            _checktimer = new DispatcherTimer();
            _checktimer.Interval = TimeSpan.FromSeconds(1);
            _checktimer.Tick += CheckTimer_Tick;
            
        }

        private void SaveSelectedContrast(int index)
        {

        }
        private void CheckTimer_Tick(object? sender, EventArgs e)
        {
            WaitDuration--;
            if (WaitDuration == 0)
            {
                _checktimer.Stop();
                BackgroundSource = black;
                //设置对比敏感度视标
                contrastSignChangeEvent.Publish(new ContrastChangeSignOptions() { PicturePath = "", Brush = SignBackGround.Black });
            }
        }
        private void Timer_Tick(object? sender, EventArgs e)
        {
            WaitDuration--;
            if (WaitDuration == 0)
            {
                _waittimer.Stop();
                _checktimer.Start();
                WaitDuration = CheckDuration;
                //设置对比敏感度视标
                GetShowPictureName();
                contrastSignChangeEvent.Publish(new ContrastChangeSignOptions() { PicturePath = _signPictureName, Brush = SignBackGround.Black });

            }
        }
        private void GetShowPictureName()
        {
            int randomIndex = Utils.GenerateRandomNumber(1, 5, imageIndex);
            imageIndex = randomIndex;
            BackgroundSource = $"df_d{imageIndex}.jpg";
            _signPictureName= $"df_t{imageIndex}.jpg";
        }



        

        



        
        public ArrowButtonStauts ArrowButtonStauts
        {
            get { return _arrowButtonStauts; }
            set { SetProperty(ref _arrowButtonStauts, value); }
        }

        public DelegateCommand ShockCommand => new DelegateCommand(Shock);

        private void Shock()
        {
            Task.Run(async () =>
            {

                ArrowButtonStauts.DownButtonStatus = true;
                await Task.Delay(50);
                ArrowButtonStauts.DownButtonStatus = false;
            });

        }

        

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            CheckDuration = navigationContext.Parameters.GetValue<int>("time");
            secondaryChangeEvent.Publish(new SecondaryChangeOptions() { Action = ChangeAction.Contrast });
            contrastSignChangeEvent.Publish(new() { PicturePath = "", Brush = SignBackGround.White });
            _waittimer.Start();
            BackgroundSource = white;


        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            secondaryChangeEvent.Publish(new() { Action = ChangeAction.Idle });
        }
    }
}
