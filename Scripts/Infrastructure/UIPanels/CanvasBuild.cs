using System.Collections;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.UIPanels.PlayerPanel;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Infrastructure.UIPanels
{
    public class CanvasBuild : MonoBehaviour
    {
        [SerializeField] private Button _mainMenuButton;
        [SerializeField] private MainPlayerInfoUI _mainPlayerInfoUI;
        [SerializeField] private RenderTexture _renderTexture;
        [SerializeField] private GameObject _renderCamera;
        private IEventBus _eventBus;
        private IMainPlayerLoader _mainPlayerLoader;

        [Inject]
        public void Construct(IEventBus eventBus, IMainPlayerLoader mainPlayerLoader)
        {
            _eventBus = eventBus;
            _mainPlayerLoader = mainPlayerLoader;
        }

        private void Start()
        {
            _mainMenuButton.onClick.AddListener(MainMenuClicked);
            _mainPlayerInfoUI.Initialize(_mainPlayerLoader);
            _mainPlayerLoader.InitializeTexture(_renderTexture);
        }

        private void MainMenuClicked()
        {
            StartCoroutine(MakeScreenShot());
        }

        private IEnumerator MakeScreenShot()
        {
            yield return new WaitForEndOfFrame();
            _mainPlayerLoader.MakeScreenshot();
            _eventBus.Invoke(new LevelStart());
        }
    }
}