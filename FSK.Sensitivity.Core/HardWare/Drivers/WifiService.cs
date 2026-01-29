using FSK.Sensitivity.Core.HardWare.Peripherals;
using FSK.Sensitivity.Core.Infrastructure;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<WifiService> logger;

        public WifiService(ILogger<WifiService> logger)
        {
            this.logger = logger;
        }

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
                        logger.LogInformation("网络已连接");
                        return true;
                    }else
                    {
                        logger.LogInformation("网络未连接");
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
