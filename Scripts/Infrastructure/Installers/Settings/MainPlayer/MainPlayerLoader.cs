using System;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Players;
using UnityEngine;

namespace Infrastructure.Installers.Settings.MainPlayer
{
    public class MainPlayerLoader : IMainPlayerLoader
    {
        private MainPlayerSetting _setting;
        private PlayerSettings _playerSettings;
        private IEventBus _eventBus;
        private RenderTexture _renderTexture;
        private bool _screenIsSaved;

        public string PlayerName => _setting.PlayerName;
        public string PlayerAvatar => _playerSettings.Avatars[_setting.PlayerAvatar];
        public int PlayerCoins => _setting.PlayerCoins;
        public int PlayerEnergy => _setting.PlayerEnergy;
        public string PlayerID => _setting.PlayerID;
        public event Action<int> ChangeCoins;
        public event Action<string, int> ChangePlayerInfo;

        public int PlayerIndexAvatar => _setting.PlayerAvatar;

        public MainPlayerLoader(PlayerSettings playerSettings, IEventBus eventBus)
        {
            _playerSettings = playerSettings;
            _eventBus = eventBus;
          
            _setting = new MainPlayerSetting(playerSettings.IdLenght);
            _setting.SetCoins(playerSettings.StartPlayerCoinsValue);
            _eventBus.Subscribe<FinishedBuild>(OnPayForBuild);
            _eventBus.Subscribe<BuyBooster>(OnBuyBooster);
        }

        public void SetPlayerName(string inputNameText)
        {
            _setting.SetPlayerName(inputNameText);
          //  ChangePlayerInfo?.Invoke(_setting.PlayerName, _setting.PlayerAvatar);
        }

        public void SetPlayerAvatar(int avatarIndex)
        {
            _setting.SetAvatar(avatarIndex);
           // ChangePlayerInfo?.Invoke(_setting.PlayerName, _setting.PlayerAvatar);
        }

        public bool ScreenshotSaved()
        {
            return _setting.ScreenshotSaved();
        }

        public void AddPlayerCoins(Players.Players currentPlayer, int indexPlayer)
        {
            if (currentPlayer.Name == PlayerName)
            {
                Debug.Log("ComPLETE QUIZ AD TO MAIN");
                var coins = _setting.PlayerCoins +
                            _playerSettings.Rewards[indexPlayer].Reward;
              
                _setting.SetCoins(coins);
            }
        }

        public void MakeScreenshot()
        {
            _screenIsSaved = true;
            _setting.MakeScreenshot(_renderTexture);
        }

        public void InitializeTexture(RenderTexture renderTexture)
        {
            _renderTexture = renderTexture;
        }

        public Texture2D GetScreenshot(int width, int height)
        {
            return _setting.GetScreenshot(width, height);
        }

        public void AddPlayerEnergy(int energi)
        {
            var energy = _setting.PlayerEnergy + energi;
            _setting.SetEnergy(energy);
        }

        private void OnPayForBuild(FinishedBuild obj)
        {
            var coins = _setting.PlayerCoins - obj.Prize;

            //edit add to another soft money
            var reward = obj.Reward;
            coins += reward;
            ChangeCoins?.Invoke(coins);
            _setting.SetCoins(coins);
        }

        private void OnBuyBooster(BuyBooster obj)
        {
            var coins = _setting.PlayerCoins - obj.Booster.Prize;
            ChangeCoins?.Invoke(coins);
            _setting.SetCoins(coins);
        }
    }
}