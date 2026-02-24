using FSK.Sensitivity.Core.Const;
using FSK.Sensitivity.Core.Enums;
using FSK.Sensitivity.Main.Controls;
using SqlSugar;
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

        public DialogCloseListener RequestClose { get; }

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

        public DelegateCommand ResultCommand =>new DelegateCommand(Result);

        private void Result()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.OK));
        }

        public DelegateCommand BackCommand => new DelegateCommand(Back);
        private void Back()
        {
            RequestClose.Invoke(new DialogResult(ButtonResult.No));
            
        }

    }
}
