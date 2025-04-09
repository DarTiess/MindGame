using System;
using System.Collections.Generic;
using GoogleSheets;

namespace Infrastructure.Installers.Settings.Network
{
    [Serializable]
    public class NetworkSettings
    {
        public List<NetworkAddressSheet> NetworkAddressSheet=new List<NetworkAddressSheet>();
    }
}