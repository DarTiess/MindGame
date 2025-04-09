using System.Collections.Generic;
using UnityEngine;


namespace Infrastructure.Installers.Settings.Bots
{
    [CreateAssetMenu(fileName = "ShopSettings", menuName = "Settings/ShopSettings")]
    public class BoosterSettings: ScriptableObject
    {
        [SerializeField] private List<BoosterConfig> _boosterConfigs;

        public List<BoosterConfig> BoosterConfigs => _boosterConfigs;
    }
}