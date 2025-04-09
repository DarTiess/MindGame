using System;
using System.Collections.Generic;
using GoogleSheets;

namespace Infrastructure.Installers.Settings.Bots
{
    [Serializable]
    public class BotsSettings
    {
        public List<BotsSheets> BotsSheetsList=new List<BotsSheets>();
    }
}