using System;
using System.Collections.Generic;
using GoogleSheets;
using Infrastructure.Installers.Settings.Players;

namespace Infrastructure.Installers.Settings.Build
{
    [Serializable]
    public class InventoryConfig
    {
        public List<ShopConfig> Boosters;
      
        public InventoryConfig()
        {
            Boosters = new List<ShopConfig>();
        }
    }
}