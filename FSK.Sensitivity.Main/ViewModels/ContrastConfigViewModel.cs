using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.HardWare.Drivers;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Core.Utility;
using Microsoft.Extensions.Logging;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FSK.Sensitivity.Main.ViewModels
{
   
    public class ContrastConfigViewModel : BaseViewModel, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogService dialogService;
        private readonly IMotor motor;
        private readonly ISpeechService speechService;
        private readonly ITrainingAndCheckService trainingAndCheckService;
        private readonly ILogger<ContrastConfigViewModel> logger;
        

        public ContrastConfigViewModel(IRegionManager regionManager, IEventAggregator eventAggregator
            , IDialogService dialogService, IMotor motor, ISpeechService speechService
            , ITrainingAndCheckService trainingAndCheckService
            ,ILogger<ContrastConfigViewModel> logger)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.motor = motor;
            this.speechService = speechService;
            this.trainingAndCheckService = trainingAndCheckService;
            this.logger = logger;
            InitHardWareChecker();
        }

        private ContrastConfigParam contrastConfigParam;
        public ContrastConfigParam ContrastConfigParam
        {
            get { return contrastConfigParam; }

            set { SetProperty(ref contrastConfigParam, value); }
        }

        private DispatcherTimer _statusCheckTimer;
        private const int TIMEOUT_SECONDS = AppConst.WaitHardwareMotionTimeout;
        private int _totalWaitTime = 0;
        private const int CHECK_INTERVAL_MS = 500; // 每500ms查询一次

        /// <summary>
        /// 开始检查硬件是否准备好
        /// </summary>
        private void StartCheckHardWareReady()
        {
            IsLoading = true;
            LoadingMessageText="硬件初始化中，请稍后...";
            _totalWaitTime = 0;
            _statusCheckTimer.Start();

        }

        private void InitHardWareChecker()
        {
            _statusCheckTimer = new DispatcherTimer();
            _statusCheckTimer.Interval = TimeSpan.FromMilliseconds(CHECK_INTERVAL_MS);
            _statusCheckTimer.Tick += async (s, e) => await OnStatusCheckTick();
            _statusCheckTimer.Stop();
        }

        private async Task OnStatusCheckTick()
        {
            logger.LogInformation("OnStatusCheckTick====================================start");
            _totalWaitTime += CHECK_INTERVAL_MS;
            if (_totalWaitTime > TIMEOUT_SECONDS * 1000)
            {
                _statusCheckTimer.Stop();
                IsLoading = false;
            }
            bool isallstop = await motor.IsAllStop();
            if (isallstop)
            {
                _statusCheckTimer.Stop();
                IsLoading = false;
            }
            logger.LogInformation("OnStatusCheckTick====================================end");
        }



        public DelegateCommand SaveCommand=> new DelegateCommand( () =>  Save());

        private void Save()
        {
             NavigateToNextPage();
        }

        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            regionManager.Regions[AppConst.MainRegion].NavigationService.Journal.GoBack();
        }

        public DelegateCommand<ItemsType?> ShowItemsDialogCommand => new DelegateCommand<ItemsType?>(ShowItemsDialog);

        private void ShowItemsDialog(ItemsType? itemsType)
        {
            ItemsType paramtype = itemsType ?? ItemsType.Eyes;

            dialogService.ShowDialog(AppConst.Main_Dialog_ShowItems, new DialogParameters() { { "ItemsType", paramtype } }, async result =>
            {

                if (result.Result == ButtonResult.OK)
                {

                    ItemsType resulttype = result.Parameters.GetValue<ItemsType>(nameof(ItemsType));
                    ShowItemsModel showItemsModel = result.Parameters.GetValue<ShowItemsModel>("selectedOption");
                    if (resulttype == ItemsType.DarkTime)
                    {
                        string newvalue = "T" + showItemsModel.Value;
                        bool res = Enum.TryParse<DCKTime>(newvalue, out DCKTime time);
                        if (res)
                        {
                            ContrastConfigParam.CheckDuration = time;
                        }
                        else
                        {
                            ContrastConfigParam.CheckDuration = DCKTime.T05;
                        }
                    }
                    else  
                    {
                        ContrastConfigParam.PD = int.Parse(showItemsModel.Value);
                         await SetHardWarePD();
                        StartCheckHardWareReady();
                    }

                }

            });
        }


        /// <summary>
        /// 设置硬件的瞳距
        /// </summary>
        private async Task SetHardWarePD()
        {
            await motor.SetSlideBlock((short)ContrastConfigParam.PD);
        }

        

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
            ContrastConfigParam = new ContrastConfigParam()
            {
                CheckDuration = DCKTime.T05,
                PD = 50
            };
            
            _ = Task.Run(async () =>
            {

                await SetHardWarePD();
                await motor.SetLeftDisk(3);
                await motor.SetRightDisk(3);
                StartCheckHardWareReady();


            });

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }

        

        // 界面跳转逻辑
        private void NavigateToNextPage()
        {
            
            NavigationParameters parameters = new NavigationParameters() ;
            parameters.Add(nameof(ContrastConfigParam), ContrastConfigParam);
            parameters.Add(nameof(TrainEnterMode), TrainEnterMode.Normal);
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_ContrastTraining, parameters);
        }
    }
}
