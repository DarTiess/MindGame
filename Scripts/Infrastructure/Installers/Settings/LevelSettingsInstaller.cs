using Infrastructure.Level;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.Settings
{
    [CreateAssetMenu(fileName = "LevelSettingsInstaller", menuName = "Installers/LevelSettingsInstaller")]
    public class LevelSettingsInstaller : ScriptableObjectInstaller<LevelSettingsInstaller>
    {
        public LevelSettings LevelSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(LevelSettings);
        }
    }
}