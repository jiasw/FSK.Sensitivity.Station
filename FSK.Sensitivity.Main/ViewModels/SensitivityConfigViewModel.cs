using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Model;
using FSK.Sensitivity.Main.Controls;
using Prism.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FSK.Sensitivity.Main.ViewModels
{
    
    public class SensitivityConfigViewModel : BaseViewModel,INavigationAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;
        private readonly ILight light;
        private readonly IMotor motor;
        private SensitivityConfigParam sensitivityConfigParam;
        //检查距离和孔洞的对应关系
        private Dictionary<CheckDistance, short> distanceDict = new Dictionary<CheckDistance, short>();


        public SensitivityConfigViewModel(IRegionManager regionManager, IDialogService dialogService,ILight light, IMotor motor )
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            this.light = light;
            this.motor = motor;
            distanceDict = new Dictionary<CheckDistance, short>() { 
                { CheckDistance.Short, 4 }, 
                { CheckDistance.Medium, 3 }, 
                { CheckDistance.Long, 2 } ,
                { CheckDistance.VeryLong, 1 } ,
            };
        }


        /// <summary>
        /// 设置硬件的瞳距
        /// </summary>
        private void SetHardWarePD()
        {
            if (sensitivityConfigParam.PD < 50 || sensitivityConfigParam.PD > 80)
            {
                return;
            }
            short pd = (short)(sensitivityConfigParam.PD - 50);
            motor?.SetSlideBlock(pd);
        }

        private void SetHardWareLight()
        {
            if (sensitivityConfigParam.IsLightOn == LightStatus.Strong)
            {
                light?.TurnOnAll();
            }
            else
            {
                light?.TurnOffAll();
            }
        }

        /// <summary>
        /// 设置硬件检查距离
        /// </summary>
        private void SetHardWareDistance()
        {
            if (sensitivityConfigParam.Eyes == Eye.OD)
            {
                motor.SetLeftDisk(0);
                motor.SetRightDisk(distanceDict[sensitivityConfigParam.CheckDistance]);
            }
            else if (sensitivityConfigParam.Eyes == Eye.OS)
            {
                motor.SetLeftDisk(distanceDict[sensitivityConfigParam.CheckDistance]);
                motor.SetRightDisk(0);
            }
            else
            {
                motor.SetRightDisk(distanceDict[sensitivityConfigParam.CheckDistance]);
                motor.SetLeftDisk(distanceDict[sensitivityConfigParam.CheckDistance]);
            }
        }

       

        /// <summary>
        /// 初始化硬件
        /// </summary>
        private void InitHardWare()
        {
            light.TurnOffAll();
            motor.Initialize();
        }

        /// <summary>
        /// 初始化训练参数
        /// </summary>
        private void InitParam()
        {
            SensitivityConfigParam = new SensitivityConfigParam()
            {
                DayNight = DayOrNight.Day,
                CheckDuration = 60,
                CheckDistance = CheckDistance.Short,
                PD = 50,
                Eyes = Eye.OU,
                IsLightOn = LightStatus.Off
            };
        }

        public SensitivityConfigParam SensitivityConfigParam
        {
            get => sensitivityConfigParam;
            set => SetProperty(ref sensitivityConfigParam, value);
        }



        public DelegateCommand SaveCommand => new DelegateCommand(async () => await Save());

        private async Task Save()
        {
            // 1. 初次判断：如果已经结束，直接跳转
            if (await IsMotionFinished())
            {
                NavigateToNextPage();
                return;
            }
            IsLoading = true;
            LoadingMessageText="硬件初始化中，请稍后...";

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
                    IsLoading= false;
                    NavigateToNextPage();
                }
            }

        }

        
        private async Task<bool> IsMotionFinished()
        {
            return await motor.IsAllStop();
        }

        // 界面跳转逻辑
        private void NavigateToNextPage()
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_SensitivityTraining
                , new NavigationParameters() { { nameof(SensitivityConfigParam), SensitivityConfigParam } });
        }

        public DelegateCommand BackCommand => new DelegateCommand(Back);

        private void Back()
        {
            regionManager.Regions[AppConst.MainRegion].NavigationService.Journal.GoBack();
        }

        public DelegateCommand ListResultCommand => new DelegateCommand(ListResult);

        private void ListResult()
        {
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_CheckHistory);
        }

        public DelegateCommand<ItemsType?> ShowItemsDialogCommand => new DelegateCommand<ItemsType?>(ShowItemsDialog);

        private void ShowItemsDialog(ItemsType? itemsType)
        {
            ItemsType paramtype=itemsType?? ItemsType.Eyes;
            
            dialogService.ShowDialog(AppConst.Main_Dialog_ShowItems, new DialogParameters() { { "ItemsType", paramtype } }, result =>
            {
                
                if (result.Result == ButtonResult.OK)
                {

                    ItemsType resulttype =result.Parameters.GetValue<ItemsType>(nameof(ItemsType));
                    ShowItemsModel showItemsModel = result.Parameters.GetValue<ShowItemsModel>("selectedOption");
                    if (resulttype == ItemsType.Eyes)
                    {
                        Eye eye = (Eye)Enum.Parse(typeof(Eye), showItemsModel.Value);
                        SensitivityConfigParam.Eyes = eye;
                        if (eye == Eye.OU)
                        {
                            SensitivityConfigParam.CheckDuration = 60;
                        }
                        else
                        {
                            SensitivityConfigParam.CheckDuration = 30;
                        }
                        SetHardWareDistance();
                    }
                    else if (resulttype==ItemsType.DayTypes)
                    {
                        DayOrNight dayNight = (DayOrNight)Enum.Parse(typeof(DayOrNight), showItemsModel.Value);
                        SensitivityConfigParam.DayNight = dayNight;
                    }
                    else if (resulttype == ItemsType.Distance)
                    {
                        CheckDistance checkDistance = (CheckDistance)Enum.Parse(typeof(CheckDistance), showItemsModel.Value);
                        SensitivityConfigParam.CheckDistance = checkDistance;
                        SetHardWareDistance();
                    }
                    else if (resulttype == ItemsType.PD)
                    {
                        SensitivityConfigParam.PD = int.Parse(showItemsModel.Value);
                        SetHardWarePD();
                    }
                    
                }

            });
        }


        //切换灯光命令
        public DelegateCommand SwitchLightCommand => new DelegateCommand(SwitchLight);

        private void SwitchLight()
        {
            SetHardWareLight();
        }



        public void OnNavigatedTo(NavigationContext navigationContext)
        {

            InitParam();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            InitHardWare();
            InitParam();
        }
    }
}
