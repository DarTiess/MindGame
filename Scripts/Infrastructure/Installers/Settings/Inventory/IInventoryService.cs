using System.Collections.Generic;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Bots;
using Infrastructure.Installers.Settings.Players;

namespace Infrastructure.Installers.Settings.Build
{
    public interface IInventoryService
    {
        List<Booster> Boosters { get; }
        void Initialize(IEventBus eventBus, BoosterSettings boosterSettings);
        void AddNewBooster(BuyBooster obj);
        void LoadSaves();
    }
}