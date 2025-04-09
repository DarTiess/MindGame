using System;
using System.Collections.Generic;
using GoogleSheets;

namespace Infrastructure.Installers.Settings.Players
{
    [Serializable]
    public class CoreSettings
    {
        public List<CoreSheets> CoreSheetsList = new List<CoreSheets>();
    }
}