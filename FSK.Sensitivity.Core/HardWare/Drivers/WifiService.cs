using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using SimpleWifi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;

namespace FSK.Sensitivity.Core.HardWare.Drivers
{
    public class WifiService:Iwifi
    {
       
        public IEnumerable<string> GetAvailableNetworks()
        {
            Wifi wifi = new Wifi();

            
            var apList = wifi.GetAccessPoints()
                .Where(n => !string.IsNullOrWhiteSpace(n.Name))
                .Select(n => n.Name);
            return apList;
        }

        public  bool Connect(string name,string password, Action<bool> onConnectComplete)
        {
            Wifi wifi = new Wifi();
            // get list of access points
            IEnumerable<AccessPoint> accessPoints = wifi.GetAccessPoints();

            // for each access point from list
            foreach (AccessPoint ap in accessPoints)
            {

                Console.WriteLine("ap: {0}\r\n", ap.Name);
                
                if (ap.Name == name)
                {
                    
                    if (ap.IsConnected)
                    {
                        LogHelper.Instance.LogInformation("网络已连接");
                        return true;
                    }else
                    {
                        LogHelper.Instance.LogInformation("网络未连接");
                        AuthRequest authRequest = new AuthRequest(ap);
                        authRequest.Password = password;
                        ap.ConnectAsync(authRequest, true, onConnectComplete);
                        return true;
                    }
                }
            }
            return false;
        }

    }
}
