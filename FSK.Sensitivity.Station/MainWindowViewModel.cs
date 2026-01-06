using FSK.Sensitivity.Core.Const;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Navigation;
using Prism.Mvvm;
using Prism.Commands;
using Prism.Navigation.Regions;
using Prism.Ioc;

namespace FSK.Sensitivity.Station
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IRegionManager regionManager;
        private readonly IContainerProvider containerProvider;

        public MainWindowViewModel( IRegionManager regionManager,IContainerProvider containerProvider)
        {
            
            this.regionManager = regionManager;
            this.containerProvider = containerProvider;
        }


        public DelegateCommand LoadCommand => new DelegateCommand(Load);

        private void Load()
        {
            var secondaryWindow = containerProvider.Resolve<SecondaryWindow>();
            secondaryWindow.Show();
            secondaryWindow.Left = 0;
            
            regionManager.RequestNavigate(AppConst.MainRegion, AppConst.Main_Page_Menu);
            
            
        }
    }
}
