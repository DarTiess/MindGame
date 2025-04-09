using Infrastructure.Installers.Settings.Bots;
using Infrastructure.Installers.Settings.Network;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.Installers.Settings.Quiz;
using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.Settings
{
    [CreateAssetMenu(fileName = "QuizSettingsInstaller", menuName = "Installers/QuizSettingsInstaller")]
    public class QuizSettingsInstaller : ScriptableObjectInstaller<QuizSettingsInstaller>
    {
        public PlayerSettings PlayerSettings;
        public QuestionsSettings Questions;
        public BotsConfig BotsConfig;
        public NetworkAddress NetworkAddress;
        public override void InstallBindings()
        {
           Container.BindInstance(Questions);
           Container.BindInstance(NetworkAddress);
           Container.BindInstance(BotsConfig);
           Container.BindInstance(PlayerSettings);
        }
    }
}