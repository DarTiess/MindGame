using UnityEngine;
using Zenject;

namespace Infrastructure.Installers.Settings.Quiz
{
    [CreateAssetMenu(fileName = "QuestionsSettingsInstaller", menuName = "Installers/QuestionsSettingsInstaller")]
    public class QuestionsSettingsInstaller : ScriptableObjectInstaller<QuestionsSettingsInstaller>
    {
       public QuestionsSettings Questions;
        public override void InstallBindings()
        {
            Container.BindInstance(Questions);
        }
    }
}