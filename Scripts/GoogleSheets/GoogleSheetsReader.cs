using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Infrastructure.Installers.Settings.Quiz;
using Leopotam.GoogleDocs.Unity;
using NaughtyAttributes;
using UnityEngine;

namespace GoogleSheets
{
    public class GoogleSheetsReader : MonoBehaviour
    {
        private Dictionary<string, string> _tagsCsvMap;
        private int _version;

        public event Action OnComlete;

        [Button]
        public void ReadDataAnyway()
        {
            string path;
#if UNITY_EDITOR
            path = $"Assets/Resources/GameData/GameDTO.json";
#else
            path = Application.persistentDataPath+$"{Constants.GAME_DTO}.json";
#endif
            if (File.Exists(path))
            {
                Debug.Log("Has json allready");
                OnComlete?.Invoke();
                return;
            }
            else
            {
                ReadGameData(false);
            }
        }

        public async Task ReadGameData(bool checkVersionFile = true)
        {
            GameDTO gameDto = new GameDTO();
          //  await ReadCsvVersion(ParseConst.Version);

          //  if (checkVersionFile)
          //  {
             //   if (ParseHelper.IsNewVersionBiggerThanOldForUnity(_version) == false)
                //    return;
          //  }

          await ReadCsvData(ParseConst.NetworkAddressSheet);
          gameDto.NetworkAddressSheet = ParseNetworkAddressData();
          Debug.Log("parse network address successfully");
          
          await ReadCsvData(ParseConst.BotsSheets);
          gameDto.BotsSheetsList = ParseBotsSheetsData();
          Debug.Log("parse bots successfully");

          await ReadCsvData(ParseConst.Boosters);
          gameDto.ShopSheetsList = ParseShopsData();
          Debug.Log("parse shop successfully");
          
          await ReadCsvData(ParseConst.Core);
          gameDto.CoreSheetsList = ParseCoreData();
          Debug.Log("parse core successfully"); 
          
          await ReadCsvData(ParseConst.Energy);
          gameDto.EnergySheetsList = ParseEnergyData();
          Debug.Log("parse energy successfully");
          
          await ReadCsvData(ParseConst.QuestionSheets);
          gameDto.QuestionSheetsList = ParseQuestionsData();
          Debug.Log("parse questions successfully");


          ParseHelper.SaveObjectToResourcesInJson(gameDto);
            OnComlete?.Invoke();
        }

        private List<BotsSheets> ParseBotsSheetsData()
        {
            Dictionary<string, List<string>> baseStatsMap = ParseHelper.GetKeyedListsByOwner(_tagsCsvMap[ParseConst.BaseStat]);
            List<BotsSheets> waveConfigData = (List<BotsSheets>)ParseHelper.WriteDataInListByType(typeof(BotsSheets), baseStatsMap, _tagsCsvMap);
            return waveConfigData;
        }

        private List<NetworkAddressSheet> ParseNetworkAddressData()
        {
            Dictionary<string, List<string>> baseStatsMap = ParseHelper.GetKeyedListsByOwner(_tagsCsvMap[ParseConst.BaseStat]);
            List<NetworkAddressSheet> waveConfigData = (List<NetworkAddressSheet>)ParseHelper.WriteDataInListByType(typeof(NetworkAddressSheet), baseStatsMap, _tagsCsvMap);
            return waveConfigData;
        }

        private List<QuestionSheets> ParseQuestionsData()
        {
            Dictionary<string, List<string>> baseStatsMap = ParseHelper.GetKeyedListsByOwner(_tagsCsvMap[ParseConst.BaseStat]);
            List<QuestionSheets> waveConfigData = (List<QuestionSheets>)ParseHelper.WriteDataInListByType(typeof(QuestionSheets), baseStatsMap, _tagsCsvMap);
            return waveConfigData;
        }

        private List<ShopConfig> ParseShopsData()
        {
            Dictionary<string, List<string>> baseStatsMap = ParseHelper.GetKeyedListsByOwner(_tagsCsvMap[ParseConst.BaseStat]);
            List<ShopConfig> waveConfigData = (List<ShopConfig>)ParseHelper.WriteDataInListByType(typeof(ShopConfig), baseStatsMap, _tagsCsvMap);
            return waveConfigData;
        }

        private List<CoreSheets> ParseCoreData()
        {
            Dictionary<string, List<string>> baseStatsMap = ParseHelper.GetKeyedListsByOwner(_tagsCsvMap[ParseConst.BaseStat]);
            List<CoreSheets> waveConfigData = (List<CoreSheets>)ParseHelper.WriteDataInListByType(typeof(CoreSheets), baseStatsMap, _tagsCsvMap);
            return waveConfigData;
        }

        private List<EnergySheetsList> ParseEnergyData()
        {
            Dictionary<string, List<string>> baseStatsMap = ParseHelper.GetKeyedListsByOwner(_tagsCsvMap[ParseConst.BaseStat]);
            List<EnergySheetsList> waveConfigData = (List<EnergySheetsList>)ParseHelper.WriteDataInListByType(typeof(EnergySheetsList), baseStatsMap, _tagsCsvMap);
            return waveConfigData;
        }

        private async Task ReadCsvVersion(string pageId)
        {
            (int version, string err) =
                await GoogleDocsLoader.LoadCsvVersion("https://docs.google.com/spreadsheets/d/1qlm3IKGUKSjLC-9NIbBbe7GAJ95Cu4MyFgvzzdtQoGw/edit?usp=sharing",
                                                      pageId);
            if (err != default)
                Debug.Log(err);
            else
                _version = version;
        }

        private async Task ReadCsvData(string pageId)
        {
            (Dictionary<string, string> csv, string err) =
                await GoogleDocsLoader.LoadFullPage(pageId, ParseConst.DividerTag);

            if (err != default)
                Debug.Log(err);
            else
                _tagsCsvMap = csv;
        }
    }
}
