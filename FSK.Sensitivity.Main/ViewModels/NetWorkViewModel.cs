using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Main.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

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
        private readonly Iwifi iwifi;

        public NetWorkViewModel(Iwifi iwifi)
        {
            this.iwifi = iwifi;
        }

        public string WifiPassword
        {
            get { return _wifiPassword; }
            set { SetProperty(ref _wifiPassword, value); }
        }

        public DelegateCommand LoadCommand=> new DelegateCommand(Load);

        private void Load()
        {
            WifiNames = iwifi.GetAvailableNetworks().ToList();
            if (WifiNames.Count > 0)
            {
                SelectedWifi = WifiNames.First();
            }

            
        }

        public DelegateCommand ConnectCommand => new DelegateCommand(Connect);
        private void Connect()
        {
           

            
            }


    }
}
