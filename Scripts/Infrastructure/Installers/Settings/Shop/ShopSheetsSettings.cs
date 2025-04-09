using System;
using System.Collections.Generic;
using GoogleSheets;

namespace Infrastructure.Installers.Settings.Shop
{
    [Serializable]
    public class ShopSheetsSettings
    {
        public List<ShopConfig> ShopSheetsList = new List<ShopConfig>();
    }
}