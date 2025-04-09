using System;
using System.IO;
using GoogleSheets;
using Network;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Infrastructure.Installers.Settings.Bots
{
    [Serializable]
    public class BotsConfig
    {
        private BotsSettings _botsSettings;

        public void SetBotsConfigs()
        {
            string json;
            string paths =  Application.persistentDataPath+$"{Constants.GAME_DTO}.json";
            if (File.Exists(paths))
            {
                json = File.ReadAllText(paths);
                Debug.Log("Applic file read");
            }
            else
            {
                json=Resources.Load<TextAsset>("GameData/"+Constants.GAME_DTO).text;
                Debug.Log("Resource file read");

            }
            Debug.Log("Set BotsConfigs"); 
            _botsSettings= JsonUtility.FromJson<BotsSettings>(json);
           
        }

        public BotsSheets GetRandomName()
        {
            var rnd = Random.Range(0, _botsSettings.BotsSheetsList.Count);
            return _botsSettings.BotsSheetsList[rnd];
        }
    }
}