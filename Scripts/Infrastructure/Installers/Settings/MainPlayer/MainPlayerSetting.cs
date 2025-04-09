using System.IO;
using System.Linq;
using Network;
using UnityEngine;
using Random = System.Random;

namespace Infrastructure.Installers.Settings.MainPlayer
{
    public class MainPlayerSetting
    {
        private string _playerName;
        private int _playerAvatar;
        private int _playerCoins;
        private string _playerID;
        private int _idLenght;
        private int _playerEnergy;

        public string PlayerName => _playerName;
        public int PlayerAvatar => _playerAvatar;
        public int PlayerCoins => _playerCoins;
        public int PlayerEnergy => _playerEnergy;
        public string PlayerID => _playerID;

        public MainPlayerSetting(int playerSettingsIdLenght)
        {
            _playerName=PlayerPrefs.GetString("MainPlayerName");
            _playerAvatar=PlayerPrefs.GetInt("MainPlayerAvatar");
            _playerCoins=PlayerPrefs.GetInt("MainPlayerCoins");
            _playerEnergy=PlayerPrefs.GetInt("MainPlayerEnergy");
            _playerID = PlayerPrefs.GetString("MainPlayerID");
            _idLenght = playerSettingsIdLenght;
                //  _playerCoins=1200;
        }
        public void SetPlayerName(string getString)
        {
            _playerName = getString;
            PlayerPrefs.SetString("MainPlayerName", _playerName);
            if (_playerID =="")
            {
                _playerID = RandomString(_idLenght);
                PlayerPrefs.SetString("MainPlayerID", _playerID);
            }
            
        }

        private string RandomString(int length)
        {
            Random rand = new Random();
            string charbase = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Range(0,length)
                .Select(_ => charbase[rand.Next(charbase.Length)])
                .ToArray());
        }
        public void SetAvatar(int getInt)
        {
            _playerAvatar = getInt;
            PlayerPrefs.SetInt("MainPlayerAvatar", _playerAvatar);
        }

        public void SetCoins(int coins)
        {
            _playerCoins = coins;
            PlayerPrefs.SetInt("MainPlayerCoins", _playerCoins);
        } 
        public void SetEnergy(int energy)
        {
            _playerEnergy = energy;
            PlayerPrefs.SetInt("MainPlayerEnergy", _playerEnergy);
            Debug.Log(_playerEnergy+" = ENERGY ");
        }

        public void MakeScreenshot(RenderTexture renderTexture)
        {
            Texture2D newScreenShot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.ARGB32, false);
            RenderTexture.active = renderTexture;
            newScreenShot.ReadPixels(new Rect(0,0,renderTexture.width, renderTexture.height), 0, 0);
            newScreenShot.Apply();
#if UNITY_EDITOR
            string path =  $"Assets/Resources/GameData/{Constants.SCREENSHOT}.png";

#else
           string path = Application.persistentDataPath + "/" + Constants.SCREENSHOT + ".png";
#endif

            
            byte[] bytes = newScreenShot.EncodeToPNG();
            
            File.WriteAllBytes(path,bytes);
        }

        public Texture2D GetScreenshot(int width, int height)
        {
            Texture2D texture2D = new Texture2D(width, height);
#if UNITY_EDITOR
            string path =  $"Assets/Resources/GameData/{Constants.SCREENSHOT}.png";

#else
           string path = Application.persistentDataPath + "/" + Constants.SCREENSHOT + ".png";
#endif
            byte[] bytes = File.ReadAllBytes(path);
            texture2D.LoadImage(bytes);
            texture2D.Apply();

            return texture2D;
        }

        public bool ScreenshotSaved()
        {
#if UNITY_EDITOR
            string path =  $"Assets/Resources/GameData/{Constants.SCREENSHOT}.png";

#else
           string path = Application.persistentDataPath + "/" + Constants.SCREENSHOT + ".png";
#endif
            return File.Exists(path);
        }
    }
}