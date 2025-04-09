using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using UnityEngine.SceneManagement;

namespace Infrastructure.Level
{
    public class LevelLoader
    {
        private readonly LevelSettings _settings;
        private readonly IEventBus _eventBus;

        public LevelLoader(LevelSettings settings, IEventBus eventBus)
        {
            _settings = settings;
            _eventBus = eventBus;
            _eventBus.Subscribe<LoadNetwork>(LoadNetworkScene);
            _eventBus.Subscribe<LevelStart>(StartGame);
            _eventBus.Subscribe<BootCoreScene>(LoadCoreScene);
            _eventBus.Subscribe<BootBuildScene>(LoadBuildScene);
        }

        public void LoadBuildScene()
        {
            SceneManager.LoadScene(_settings.BuildScene);
        }

        private void LoadNetworkScene(LoadNetwork obj)
        {
            SceneManager.LoadScene(_settings.NetworkScene);
        }

        private void LoadBuildScene(BootBuildScene obj)
        {
            LoadBuildScene();
        }

        private void LoadCoreScene(BootCoreScene obj)
        {
            SceneManager.LoadScene(_settings.CoreScene);
        }

        private void StartGame(LevelStart obj)
        {
            SceneManager.LoadScene(_settings.BaseScene);
        }
    }
}