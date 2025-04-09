using System;
using System.Collections.Generic;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Bots;
using Infrastructure.Installers.Settings.Players;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Build
{
    [Serializable]
    public class InventoryService : IInventoryService
    {
        private InventorySetting _setting;
        private BoosterSettings _boosterSettings;
        private IEventBus _eventBus;
        public List<Booster> Boosters => _setting.Boosters;
        
        public void Initialize(IEventBus eventBus, BoosterSettings boosterSettings)
        {
            Debug.Log("Init Inventory");
            _eventBus = eventBus;
            _eventBus.Subscribe<BuyBooster>(AddNewBooster);
            _boosterSettings = boosterSettings;
            _setting = new InventorySetting(_boosterSettings);
            _setting.Load();
        }

        public void AddNewBooster(BuyBooster obj)
        {
            _setting.AddNewBooster(obj.Booster);
            _setting.SaveChanges();
        }

        public void LoadSaves()
        {
            if (_setting == null)
                _setting = new InventorySetting(_boosterSettings);
            _setting.Load();
        }
      
    }
}