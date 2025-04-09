using System;
using System.Collections.Generic;
using System.IO;
using GoogleSheets;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.Installers.Settings.Shop;
using Network;
using UnityEngine;
using UnityEngine.Serialization;

namespace Infrastructure.Installers.Settings.Bots
{
    [Serializable]
    public class ShopsService
    {
        [SerializeField] private BoosterSettings boosterSettings;
        [SerializeField] private ShopSheetsSettings _shopSheetsSettings;
        private List<Booster> _boosters;
        public List<Booster> Boosters => _boosters;
        public BoosterSettings BoosterSettings => boosterSettings;

        public void SetShopConfigs()
        {
            string json;
            string paths = Application.persistentDataPath + $"{Constants.GAME_DTO}.json";
            if (File.Exists(paths))
            {
                json = File.ReadAllText(paths);
                Debug.Log("Applic file read");
            }
            else
            {
                json = Resources.Load<TextAsset>("GameData/" + Constants.GAME_DTO).text;
                Debug.Log("Resource file read");
            }

            Debug.Log("Set ShopConfigs");
            _shopSheetsSettings = JsonUtility.FromJson<ShopSheetsSettings>(json);

            _boosters = new List<Booster>();
            for (int i = 0; i < _shopSheetsSettings.ShopSheetsList.Count; i++)
            {
                if (_shopSheetsSettings.ShopSheetsList[i].BoosterName != "Soft")
                {
                    _boosters.Add(new Booster(
                        boosterSettings.BoosterConfigs.Find(x =>
                            x.BoosterName == _shopSheetsSettings.ShopSheetsList[i].BoosterName),
                        _shopSheetsSettings.ShopSheetsList[i].BoosterCount,
                        _shopSheetsSettings.ShopSheetsList[i].BoosterPrize,
                        _shopSheetsSettings.ShopSheetsList[i].IsOpened));
                }
               
            }
        }
    }
}