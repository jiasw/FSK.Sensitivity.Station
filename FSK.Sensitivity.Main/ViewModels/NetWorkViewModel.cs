using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Main.ViewModels
{
    public class NetWorkViewModel : BaseViewModel
    {
         List<string> _wifiNames;
        public List<string> WifiNames
        {
            get { return _wifiNames; }
            set { SetProperty(ref _wifiNames, value); }
        }


         string _selectedWifi;
        public string SelectedWifi
        {
            get { return _selectedWifi; }
            set { SetProperty(ref _selectedWifi, value); }

        }

         string _wifiPassword;
        public string WifiPassword
        {
            get { return _wifiPassword; }
            set { SetProperty(ref _wifiPassword, value); }
        }

        public DelegateCommand LoadCommand=> new DelegateCommand(Load);

        private void Load()
        {
            WifiNames = new List<string>() { "FSK-Guest","FSK-Admin","FSK-Test"};
            SelectedWifi = WifiNames.FirstOrDefault();
        }


    }
}
