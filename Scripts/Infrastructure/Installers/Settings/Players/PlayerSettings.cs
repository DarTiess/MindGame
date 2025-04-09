using System;
using System.Collections.Generic;
using System.IO;
using GoogleSheets;
using Network;
using Network.NetworkObjects;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Players
{
    [Serializable]
    public class PlayerSettings
    {
        // [SerializeField] private float _waitToStart;
       // [SerializeField] private int _timerToAnswer;
        [SerializeField] private float _freezeTimer;
      //  [SerializeField] private int _questionsAmountInCore;
        [SerializeField] private int _idLenght;
        [SerializeField] private int _startPlayerCoinsValue;
        //  [Header("Booster Settings")] [SerializeField]
        // //  private List<Booster> _boosters;
        [Header("Bots Settings")] 
        [SerializeField, Range(0, 100)] private int _botChance;
        [SerializeField, Range(0, 100)] private float _botStartChance;
        [SerializeField] private float _botStartSpeed;
        [SerializeField] private int _botAttackStartLevel;
        [SerializeField, Range(0, 100)] private float _botAttackChance;
        [Space(20)]
      //  [SerializeField] private float _loseStreak;
      //  [SerializeField] private List<WinStreak> _winStreaks;
      //  [SerializeField] private List<Rewards> _rewards;
        [SerializeField] private List<string> _avatars;
        [SerializeField] private List<Players> _playersNames;

        public CoreSheets _coreSheets;
        private string _currentPlayer;
      //  private IEventBus _eventBus;
        public List<Rewards> Rewards =>_coreSheets.Rewards;
        public string CurrentPlayer
        {
            get => _currentPlayer;
            set => _currentPlayer = value;
        }
        public int TimerToAnswer => _coreSheets.RoundTimer;
        public List<Players> PlayersNames
        {
            get => _playersNames;
            set => _playersNames = value;
        }
        public float LoseStreak => _coreSheets.LooseStreak;
        public List<WinStreak> WinStreaks =>_coreSheets.WinStreak;
        public int QuestionAmount => _coreSheets.RoundsAmount;
        public float FreezeTimer => _freezeTimer;
        public List<string> Avatars => _avatars;
        public int BotChance => _botChance;
        public float BotStartSpeed => _botStartSpeed;
        public float BotStartChance => _botStartChance;
        public int BotAttackStartLevel => _botAttackStartLevel;

        public float BotAttackChance => _botAttackChance;

        //  public List<Booster> Boosters => _boosters;
        public int IdLenght => _idLenght;
        public int StartPlayerCoinsValue => _startPlayerCoinsValue;

        public void SetCoreConfigs()
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
            Debug.Log("Set CoreConfigs "); 
            CoreSettings coreList= JsonUtility.FromJson<CoreSettings>(json);
            _coreSheets =coreList.CoreSheetsList[0];
            if (_coreSheets == null)
            {
                Debug.Log("EMPTY CORE");
            }
        }

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public static PlayerSettings Deserialize(string json)
        {
            return JsonUtility.FromJson<PlayerSettings>(json);
        }

        public void AddNewPlayer(PlayerHandler playerHandler)
        {
            Players newPlayer = new Players()
            {
                Name = playerHandler.playerName, Coins = 0,
                AvatarName = playerHandler.playerAvatarName,
                LoseStreak = 0,
                IsBot = playerHandler.isbot
            };
            _playersNames.Add(newPlayer);
        }

        public void Clear()
        {
            _playersNames.Clear();
        }
    }
}