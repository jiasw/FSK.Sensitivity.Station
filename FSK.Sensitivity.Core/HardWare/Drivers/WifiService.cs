using FSK.Sensitivity.Core.HardWare.Peripherals;
using ManagedNativeWifi;
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
            return NativeWifi.EnumerateAvailableNetworkSsids()
        .Select(x => x.ToString());
        }

        public  async Task<bool> ConnectAsync()
        {
            var availableNetwork = NativeWifi.EnumerateAvailableNetworks()
                .Where(x => !string.IsNullOrWhiteSpace(x.ProfileName))
                .OrderByDescending(x => x.SignalQuality)
                .FirstOrDefault();

            if (availableNetwork is null)
                return false;

            return await NativeWifi.ConnectNetworkAsync(
                interfaceId: availableNetwork.InterfaceInfo.Id,
                profileName: availableNetwork.ProfileName,
                bssType: availableNetwork.BssType,
                timeout: TimeSpan.FromSeconds(10));
        }

        public Task RefreshAsync()
        {
            return NativeWifi.ScanNetworksAsync(timeout: TimeSpan.FromSeconds(10));
        }
    }
}
