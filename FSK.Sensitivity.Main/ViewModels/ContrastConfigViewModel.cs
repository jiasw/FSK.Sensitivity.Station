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
        }

        private ContrastConfigParam contrastConfigParam;
        public ContrastConfigParam ContrastConfigParam
        {
            get { return contrastConfigParam; }

            set { SetProperty(ref contrastConfigParam, value); }
        }

        public DelegateCommand SaveCommand=> new DelegateCommand(async () => await Save());

        private async Task Save()
        {
           
            IsLoading = true;
            LoadingMessageText = "硬件初始化中，请稍后...";

            await Utils.WaitForConditionAsync(IsMotionFinished, NavigateToNextPage, AppConst.WaitHardwareMotionTimeout);
            IsLoading = false;
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

            dialogService.ShowDialog(AppConst.Main_Dialog_ShowItems, new DialogParameters() { { "ItemsType", paramtype } }, result =>
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
                        SetHardWarePD();
                    }

                }

            });
        }


        /// <summary>
        /// 设置硬件的瞳距
        /// </summary>
        private void SetHardWarePD()
        {
            motor?.SetSlideBlock((short)ContrastConfigParam.PD);
        }

        

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            
            ContrastConfigParam = new ContrastConfigParam()
            {
                CheckDuration = DCKTime.T05,
                PD = 50
            };
            _ = Task.Run(() =>
            {
                try
                {
                    IsLoading = true;
                    LoadingMessageText = "硬件正在初始化,请稍候...";
                    motor.Initialize();
                    SetHardWarePD();
                    motor.SetLeftDisk(3);
                    motor.SetRightDisk(3);
                }
                finally
                {
                    LoadingMessageText = "";
                    IsLoading = false;

                }

            });

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
           
        }

        private async Task<bool> IsMotionFinished()
        {
            return await motor.IsAllStop();
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
