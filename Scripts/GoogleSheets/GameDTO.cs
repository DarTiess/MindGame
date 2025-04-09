using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.Quiz;

namespace GoogleSheets
{
	[Serializable]
    public class GameDTO
    {
	    public List<NetworkAddressSheet> NetworkAddressSheet;
	    public List<BotsSheets> BotsSheetsList;
	    public List<QuestionSheets> QuestionSheetsList;
	    public List<ShopConfig> ShopSheetsList;
	    public List<CoreSheets> CoreSheetsList;
	    public List<EnergySheetsList> EnergySheetsList;
    }
}