using Infrastructure.Installers.Settings;
using Infrastructure.Installers.Settings.Players;

namespace Infrastructure.EventsBus.Signals
{
    public struct LevelLost
    {
        private readonly Players _player;
        public Players Player => _player;
        public LevelLost(Players player)
        {
            _player = player;
        }
    }
}