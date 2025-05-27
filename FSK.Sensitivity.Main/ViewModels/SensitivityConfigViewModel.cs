using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Core.Model;
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

        public SensitivityConfigViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
            
        }

        private SensitivityConfigParam sensitivityConfigParam=new SensitivityConfigParam()
        {
            DayNight = DayOrNight.Day,
            CheckDuration = 30,
            CheckDistance = CheckDistance.Short,
            PD = 55,
            Eyes = Eye.OS,
            IsLightOn = LightStatus.Off
        };

        


        public SensitivityConfigParam SensitivityConfigParam
        {
            get => sensitivityConfigParam;
            set => SetProperty(ref sensitivityConfigParam, value);
        }


        public DelegateCommand SaveCommand => new DelegateCommand(Save);

        private void Save()
        {
            regionManager.RequestNavigate(AppConst.TrainRegion, AppConst.Main_Page_SensitivityTraining);
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
                    }else if (itemsType == "2")
                    {
                        DayOrNight dayNight = (DayOrNight)Enum.Parse(typeof(DayOrNight), showItemsModel.Value);
                        SensitivityConfigParam.DayNight = dayNight;
                    }
                    else if (itemsType == "3")
                    {
                        CheckDistance checkDistance = (CheckDistance)Enum.Parse(typeof(CheckDistance), showItemsModel.Value);
                        SensitivityConfigParam.CheckDistance = checkDistance;
                    }
                    else if (itemsType == "4")
                    {
                        SensitivityConfigParam.PD = int.Parse(showItemsModel.Value);
                    }
                    
                }

            });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SensitivityConfigParam = new SensitivityConfigParam()
            {
                DayNight = DayOrNight.Day,
                CheckDuration = 30,
                CheckDistance = CheckDistance.Short,
                PD = 55,
                Eyes = Eye.OS,
                IsLightOn = LightStatus.Off
            };
            
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            var str= navigationContext.NavigatedName();
        }
    }
}
