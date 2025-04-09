using Infrastructure.Installers.Settings;
using Infrastructure.Installers.Settings.Players;

namespace Infrastructure.EventsBus.Signals
{
    public struct LevelWin
    {
        private readonly Players _player;
        public Players Player => _player;

        public LevelWin(Players player)
        {
            _player = player;
        }
    }
}