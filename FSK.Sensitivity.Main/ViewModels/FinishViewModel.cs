using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Main.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class FinishViewModel : BaseViewModel, IDialogAware
    {
        private readonly IRegionManager regionManager;
        private readonly IDialogService dialogService;

        private CheckItem backMenu=CheckItem.CSF;
        public FinishViewModel(IRegionManager regionManager, IDialogService dialogService)
        {
            this.regionManager = regionManager;
            this.dialogService = dialogService;
        }

        public DialogCloseListener RequestClose => throw new NotImplementedException();

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            backMenu=parameters.GetValue<CheckItem>("BackMenu");
        }

        public DelegateCommand PrintCommand =>new DelegateCommand(Print);

        private void Print()
        {
            MessageBoxService.Instance.Show("打印成功");
        }

        public DelegateCommand BackCommand => new DelegateCommand(Back);
        private void Back()
        {
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_Menu);
        }

    }
}
