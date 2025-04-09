using System;
using System.Collections.Generic;
using System.IO;
using GoogleSheets;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Bots;
using Infrastructure.Installers.Settings.Players;
using Network;
using Newtonsoft.Json;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Build
{
    [Serializable]
    public class InventorySetting
    {
        private InventoryConfig _inventoryConfig;
        private BoosterSettings _boosterSettings;
        private List<Booster> _boosters;
        public List<Booster> Boosters => _boosters;

        public InventorySetting(BoosterSettings boosterSettings)
        {
            _inventoryConfig = new InventoryConfig();
            _boosterSettings = boosterSettings;
        }

        public void SaveChanges()
        {
            Debug.Log("Save Inventory");

#if UNITY_EDITOR
            var jsonSer = JsonConvert.SerializeObject(_inventoryConfig, Formatting.Indented);
            string path = $"Assets/Resources/GameData/{Constants.INVENTORY_CONFIGS}.json";
            File.WriteAllText(path, jsonSer);
#else
            var jsonSer = JsonConvert.SerializeObject(_inventoryConfig, Formatting.Indented);
            string path = Application.persistentDataPath+$"{Constants.INVENTORY_CONFIGS}.json";
            File.WriteAllText(path, jsonSer);
#endif
        }

        public void Load()
        {
            Debug.Log("Load InventorySettings");
            _boosters = new List<Booster>();
            string json;
#if UNITY_EDITOR
            string paths = $"Assets/Resources/GameData/{Constants.INVENTORY_CONFIGS}.json";

#else
           string paths = Application.persistentDataPath+$"{Constants.INVENTORY_CONFIGS}.json";
#endif

            if (File.Exists(paths))
            {
#if UNITY_EDITOR
                json = Resources.Load<TextAsset>("GameData/" + Constants.INVENTORY_CONFIGS).text;
                Debug.Log("Resource file read Inventory");
#else
               json = File.ReadAllText(paths);
               Debug.Log("Applic file read Inventory");
#endif

                InventoryConfig inventoryConfig = JsonUtility.FromJson<InventoryConfig>(json);
                _inventoryConfig = inventoryConfig;

                if (_inventoryConfig == null || _inventoryConfig.Boosters == null ||
                    _inventoryConfig.Boosters.Count <= 0)
                    return;
                for (int i = 0; i < _inventoryConfig.Boosters.Count; i++)
                {
                    var booster = new Booster(
                        _boosterSettings.BoosterConfigs.Find(x =>
                            x.BoosterName == _inventoryConfig.Boosters[i].BoosterName),
                        _inventoryConfig.Boosters[i].BoosterCount,
                        _inventoryConfig.Boosters[i].BoosterPrize,
                        _inventoryConfig.Boosters[i].IsOpened);
                    _boosters.Add(booster);

                    booster.Remove += OnRemoveBooster;
                }

                Debug.Log("Complete Load Saves Inventorys");
            }
            else
            {
                CreateBoosterList();
                SaveChanges();

#if UNITY_EDITOR
                json = Resources.Load<TextAsset>("GameData/" + Constants.INVENTORY_CONFIGS)?.text;
#else
               json = File.ReadAllText(paths);
#endif
            }
        }

        public void AddNewBooster(Booster obj)
        {
            if (_boosters.Exists(x => x.BoosterName == obj.BoosterName))
            {
                Booster b = _boosters.Find(x => x.BoosterName == obj.BoosterName);
                b.Count += obj.Count;
                ShopConfig invB = _inventoryConfig.Boosters.Find(x => x.BoosterName == obj.BoosterName);
                invB.BoosterCount = b.Count;
            }
            else
            {
                CreteBooster(obj);
            }
        }

        private void CreteBooster(Booster obj)
        {
            _boosters.Add(obj);
            obj.Remove += OnRemoveBooster;
            var boost = new ShopConfig();
            boost.BoosterName = obj.BoosterName;
            boost.BoosterCount = obj.Count;
            boost.BoosterPrize = obj.Prize;
            _inventoryConfig.Boosters.Add(boost);
        }

        private void CreateBoosterList()
        {
            if (_boosters == null)
            {
                _boosters = new List<Booster>();
            }

            for (int i = 0; i < 4; i++)
            {
                var boost = new Booster(_boosterSettings.BoosterConfigs[i], 1, 0, true);
                CreteBooster(boost);
            }
        }

        private void OnRemoveBooster(Booster booster)
        {
            booster.Remove -= OnRemoveBooster;
            _boosters.Remove(booster);
            _inventoryConfig.Boosters.Remove(_inventoryConfig.Boosters.Find(x => x.BoosterName == booster.BoosterName
                && x.BoosterPrize == booster.Prize));
        }
    }
}