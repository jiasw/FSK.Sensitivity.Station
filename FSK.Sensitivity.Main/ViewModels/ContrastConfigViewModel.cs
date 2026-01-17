using FSK.Sensitivity.Core;
using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.EventBus;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Model;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace FSK.Sensitivity.Main.ViewModels
{
    [RegionMemberLifetime(KeepAlive = false)]
    public class ContrastConfigViewModel : BaseViewModel, INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IEventAggregator eventAggregator;
        private readonly IDialogService dialogService;
        private readonly IMotor motor;

        public ContrastConfigViewModel(IRegionManager regionManager, IEventAggregator eventAggregator, IDialogService dialogService, IMotor motor)
        {
            this.regionManager = regionManager;
            this.eventAggregator = eventAggregator;
            this.dialogService = dialogService;
            this.motor = motor;
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
            // 1. 初次判断：如果已经结束，直接跳转
            if (await IsMotionFinished())
            {
                NavigateToNextPage();
                return;
            }
            IsLoading = true;
            LoadingMessageText = "硬件初始化中，请稍后...";

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(AppConst.WaitHardwareMotionTimeout)))
            {
                try
                {
                    // 3. 后台轮询任务
                    await Task.Run(async () =>
                    {
                        while (!cts.Token.IsCancellationRequested)
                        {
                            if (await IsMotionFinished())
                            {
                                break; // 下位机运动结束，跳出循环
                            }
                            // 轮询间隔，防止占用 CPU 过高
                            await Task.Delay(100, cts.Token);
                        }
                    }, cts.Token);
                }
                catch (OperationCanceledException)
                {

                    Console.WriteLine("等待超时，强制跳转");
                }
                finally
                {
                    IsLoading = false;
                    NavigateToNextPage();
                }
            }

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
            if (ContrastConfigParam.PD < 50 || ContrastConfigParam.PD > 80)
            {
                return;
            }
            short pd = (short)(ContrastConfigParam.PD - 50);
            motor?.SetSlideBlock(pd);
        }

        

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ContrastConfigParam = new ContrastConfigParam()
            {
                CheckDuration = DCKTime.T05,
                PD = 50
            };
            motor.Initialize();
            SetHardWarePD();
            motor.SetLeftDisk(3);
            motor.SetRightDisk(3);
            

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //判断设备是否初始化完成
        }

        private async Task<bool> IsMotionFinished()
        {
            return await motor.IsAllStop();
        }

        // 界面跳转逻辑
        private void NavigateToNextPage()
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_ContrastTraining, new NavigationParameters() { { nameof(ContrastConfigParam), ContrastConfigParam } });
        }
    }
}
