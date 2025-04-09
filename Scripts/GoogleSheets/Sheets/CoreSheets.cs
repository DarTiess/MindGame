using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.Players;

namespace GoogleSheets
{
    [Serializable]
    public  class CoreSheets
    {
        public int RoundTimer;
        public int RoundsAmount;
        public List<Rewards> Rewards;
        public List<WinStreak> WinStreak;
        public float LooseStreak;
    }
}