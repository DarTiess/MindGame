using Infrastructure.Installers.Settings.Players;

namespace Infrastructure.EventsBus.Signals
{
    public struct BuyBooster
    {
        private Booster _booster;
        public Booster Booster => _booster;

        public BuyBooster( Booster booster)
        {
            _booster = booster;
        }
    }
}