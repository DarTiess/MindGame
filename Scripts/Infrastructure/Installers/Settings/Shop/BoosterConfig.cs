using System;
using Infrastructure.UIPanels.Boosters;
using UnityEngine;

namespace Infrastructure.Installers.Settings.Bots
{
    [Serializable]
    public class BoosterConfig
    {
        [SerializeField] private BoosterType _type;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _boosterName;
        [SerializeField] private string _description;

        public BoosterType Type => _type;
        public Sprite Icon => _icon;
        public string BoosterName => _boosterName;
        public string Description => _description;
    }
}