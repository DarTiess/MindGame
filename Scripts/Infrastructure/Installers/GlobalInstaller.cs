using Infrastructure.EventsBus;
using Infrastructure.Installers.Settings.Build;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.Level;
using Zenject;

namespace Infrastructure.Installers
{
    public class GlobalInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            CreateEventBus();
            CreateLevelLoader();
            CreateQuizHandler();
            CreateBuildService();
            CreateMainPlayerLoader();
            CreateInventoryService();
        }

        private void CreateQuizHandler()
        {
            Container.BindInterfacesAndSelfTo<QuizHandler>().AsSingle();
        }

        private void CreateEventBus()
        {
            Container.BindInterfacesAndSelfTo<EventBus>().AsSingle();
        }

        private void CreateLevelLoader()
        {
            Container.BindInterfacesAndSelfTo<LevelLoader>().AsSingle();
        }

        private void CreateMainPlayerLoader()
        {
            Container.BindInterfacesAndSelfTo<MainPlayerLoader>().AsSingle();
        }

        private void CreateBuildService()
        {
            Container.BindInterfacesAndSelfTo<BuildsService>().AsSingle();

        }

        private void CreateInventoryService()
        {
            Container.BindInterfacesAndSelfTo<InventoryService>().AsSingle();
        }

     
    }
}
