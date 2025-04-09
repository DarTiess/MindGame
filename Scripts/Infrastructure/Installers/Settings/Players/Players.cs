using System;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Players
{
    [Serializable]
    public class Players
    {
        [SerializeField] private string _name;
        [SerializeField] private float _coins=0;
        [SerializeField] private float _loseStreak;
        [SerializeField] private string _avatarName;
        [SerializeField] private int _winLevel;
        private bool _isBot=false;
        private bool _vaBanque=false;
        public string Name { get => _name; set=> _name = value;  }
        public float Coins { get => _coins; set=> _coins = value; }
        public float LoseStreak { get => _loseStreak; set=> _loseStreak =value; }
        public string AvatarName { get => _avatarName; set => _avatarName = value; }
        public bool IsBot { get=>_isBot; set=>_isBot=value; }
        public int WinLevel { get=>_winLevel; set=>_winLevel=value; }
        public bool VaBanque { get=>_vaBanque; set=>_vaBanque=value; }

        public Sprite GetAvatarSprite()
        {
            return Resources.Load<Sprite>(_avatarName);
        }

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public static Players Deserialize(string json)
        {
            return JsonUtility.FromJson<Players>(json);
        }
    }
}