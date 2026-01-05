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
            light.TurnOffRight();
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


        private async Task<bool> HardWareIsMove()
        {
            return false;
            return await motor.IsSlideMove() || await motor.IsLeftMove() || await motor.IsRightMove();
        }


        public DelegateCommand SaveCommand => new DelegateCommand(async () => await Save());

        private async Task Save()
        {
            if (await HardWareIsMove())
            {
                AlertMessageBox.Show("设备正在初始化,请稍候...");
                return;
            }

            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_SensitivityTraining
                , new NavigationParameters() { { "sensitivityConfigParam", SensitivityConfigParam } });
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

        public DelegateCommand<string> ShowItemsDialogCommand => new DelegateCommand<string>(ShowItemsDialog);

        private void ShowItemsDialog(string itemsType)
        {
            dialogService.ShowDialog(AppConst.Main_Dialog_ShowItems, new DialogParameters() { { "itemsType", itemsType } }, result =>
            {
                
                if (result.Result == ButtonResult.OK)
                {
                   
                    string itemsType=result.Parameters.GetValue<string>("itemsType");
                    ShowItemsModel showItemsModel = result.Parameters.GetValue<ShowItemsModel>("selectedOption");
                    if (itemsType == "1")
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
                    else if (itemsType == "2")
                    {
                        DayOrNight dayNight = (DayOrNight)Enum.Parse(typeof(DayOrNight), showItemsModel.Value);
                        SensitivityConfigParam.DayNight = dayNight;
                    }
                    else if (itemsType == "3")
                    {
                        CheckDistance checkDistance = (CheckDistance)Enum.Parse(typeof(CheckDistance), showItemsModel.Value);
                        SensitivityConfigParam.CheckDistance = checkDistance;
                        SetHardWareDistance();
                    }
                    else if (itemsType == "4")
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
