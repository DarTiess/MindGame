using Infrastructure.Installers.Settings.Bots;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Infrastructure.Installers.Settings
{
    [CreateAssetMenu(fileName = "ShopSettingsInstaller", menuName = "Installers/ShopSettingsInstaller")]
    public class ShopSettingInstaller : ScriptableObjectInstaller<ShopSettingInstaller>
    {
        public ShopsService shopsService;
        public override void InstallBindings()
        {
            Container.BindInstance(shopsService);
        }
    }
}