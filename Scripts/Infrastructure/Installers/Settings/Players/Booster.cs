using System;
using Infrastructure.Installers.Settings.Bots;
using Infrastructure.UIPanels.Boosters;
using UnityEngine;
using UnityEngine.Serialization;

namespace Infrastructure.Installers.Settings.Players
{
    [Serializable]
    public class Booster
    {
        [SerializeField] private BoosterConfig _boosterConfig;
        [SerializeField] private int _count;
        [SerializeField] private bool _isUsed;
        private int _prize;
        private bool _isOpened;


        public BoosterType Type => _boosterConfig.Type;
        public int Count
        {
            get => _count;
            set => _count = value;
        }


        public Sprite Icon => _boosterConfig.Icon;
        public string BoosterName => _boosterConfig.BoosterName;
        public string Description => _boosterConfig.Description;
        public int Prize => _prize;

        public bool IsUsed
        {
            get => _isUsed;
            set => _isUsed = value;
        }

        public bool IsOpened => _isOpened;

        public event Action<Booster>Remove;

        public Booster(BoosterConfig boosterConfig, int count, int prize, bool isOpened)
        {
            _boosterConfig = boosterConfig;
            _count = count;
            _isUsed = false;
            _prize = prize;
            _isOpened = isOpened;
        }

        public void RemoveBooster()
        {
            if(_count<=0)
                return;
            
            _count--;

            if (_count <= 0)
            {
                _isUsed = false;
                Remove?.Invoke(this);

            }
            //Remove from playerBoosters
        }
    }
}