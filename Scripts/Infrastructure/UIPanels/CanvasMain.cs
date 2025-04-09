using DG.Tweening;
using Infrastructure.EventsBus;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.Level;
using Infrastructure.UIPanels.Auth;
using Infrastructure.UIPanels.Inventory;
using Infrastructure.UIPanels.PlayerPanel;
using Network;
using Network.NetworkObjects;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Infrastructure.UIPanels
{
    public class CanvasMain : MonoBehaviour
    {
        // [SerializeField] private Button _openQuizButton;
        [SerializeField] private AuthenticationPanel _authentication;
        [SerializeField] private Button _enterHostButton;
        [SerializeField] private Button _enterClientButton;
        [SerializeField] private Button _buildButton;
        [SerializeField] private RawImage _background;
        [SerializeField] private Image _connectingPanel;
        [Space(20)]
        [SerializeField] private MainPlayerInfoUI _mainPlayerInfoUI;
        [SerializeField] private Button _playerProfileButton;
        [SerializeField] private PlayerProfilePanel _playerProfilePanel;
        [Space(20)]
        [SerializeField] private Button _inventoryButton;
        [SerializeField] private InventoryPanel _inventoryPanel;
        [Space(20)] 
        [SerializeField] private Button _shopButton;
        [SerializeField] private ShopPanel _shopPanel;

        
        [SerializeField] private GameObject _playButtonImage;

        private QuizNetworkManager _networkManager;
        private PlayerAuth _playerAuth;
        private IMainPlayerLoader _mainPlayerLoader;
        private IEventBus _eventBus;
        private LevelLoader _levelLoader;
        private AnimationSettings _animationSettings;

        [Inject]
        public void Construct(IMainPlayerLoader mainPlayerLoader, IEventBus eventBus, LevelLoader levelLoader, AnimationSettings animationSettings)
        {
            _mainPlayerLoader = mainPlayerLoader;
            _eventBus = eventBus;
            _levelLoader = levelLoader;
            _animationSettings = animationSettings;
        }

        private void Start()
        {
            _networkManager = FindObjectOfType<QuizNetworkManager>();
            _playerAuth = FindObjectOfType<PlayerAuth>();
            _connectingPanel.gameObject.SetActive(false);
            if (_networkManager == null || _playerAuth == null)
            {
                Debug.LogError("QuizNetworkManager or PlayerAuth not found in the scene.");
                return;
            }

            LoadPlayerConfigs();

            _mainPlayerLoader.ChangePlayerInfo += SavePlayerInfo;
            _enterHostButton.onClick.AddListener(EnterHostClicked);
            _enterClientButton.onClick.AddListener(EnterClientClicked);
            _buildButton.onClick.AddListener(BuildButtonClicked);
            _playerProfileButton.onClick.AddListener(OpenPlayerProfilePanel);
            _inventoryButton.onClick.AddListener(OpenInventoryPanel);
            _shopButton.onClick.AddListener(OpenShopPanel);
        }

        private void LoadPlayerConfigs()
        {
            if (_mainPlayerLoader.PlayerName != "")
            {
                _playerAuth.SetPlayerName(_mainPlayerLoader.PlayerName);
                _enterClientButton.gameObject.SetActive(true);
                _authentication.Hide();
                _mainPlayerInfoUI.Initialize(_mainPlayerLoader);
                SetBackgroundScreenshot();
            }
            else
            {
                _authentication.Show();
                _authentication.OnSavePlayerInfo += SavePlayerInfo;
                _mainPlayerInfoUI.Hide();
                _enterClientButton.gameObject.SetActive(false);
            }
        }

        private void OpenPlayerProfilePanel()
        {
            _playerProfilePanel.ChangedName += OnChangedName;
            _playerProfilePanel.ChangedAvatar += OnChangedAvatar;
            _playerProfilePanel.ShowPanel(_mainPlayerLoader);
        }

        private void OpenInventoryPanel()
        {
            _inventoryPanel.Show();
        }

        private void OpenShopPanel()
        {
            _shopPanel.Show();
        }

        private void OnChangedAvatar()
        {
            _mainPlayerInfoUI.Initialize(_mainPlayerLoader);
        }

        private void OnChangedName()
        {
           // _playerProfilePanel.ChangeName -= OnChangeName; ANOTHER PALCE
            _playerAuth.SetPlayerName(_mainPlayerLoader.PlayerName);
            _mainPlayerInfoUI.Initialize(_mainPlayerLoader);
        }

        private void SetBackgroundScreenshot()
        {
            if(!_mainPlayerLoader.ScreenshotSaved())
                return;
            var canvasScaler = GetComponent<CanvasScaler>();
            var width = canvasScaler.referenceResolution.x;
            var height = canvasScaler.referenceResolution.y;
            Texture2D texture2D = _mainPlayerLoader.GetScreenshot((int)width, (int)height);
            _background.texture = texture2D;
        }

        private void BuildButtonClicked()
        {
            _levelLoader.LoadBuildScene();
        }

        private void SavePlayerInfo(string name, int avatarIndex)
        {
            _playerAuth.SetPlayerName(name);
            _mainPlayerLoader.SetPlayerName(name);
            _mainPlayerLoader.SetPlayerAvatar(avatarIndex);
            _mainPlayerInfoUI.Initialize(_mainPlayerLoader);
            _enterClientButton.gameObject.SetActive(true);
        }

        private void EnterClientClicked()
        {
            _playButtonImage.transform.DOLocalMoveY(_animationSettings.PlayBtnClickedPosition, _animationSettings.PlayBtnDuration)
                .SetEase(_animationSettings.PlayBtnEase)
                .OnComplete(() =>
                {
                    _connectingPanel.gameObject.SetActive(true);
                    _networkManager.StartClient();
                });
        }

        private void EnterHostClicked()
        {
            _networkManager.StartServer();
        }

        private void MainMenuClicked()
        {
            _buildButton.gameObject.SetActive(true);
            _enterClientButton.gameObject.SetActive(true);
        }
    }
}