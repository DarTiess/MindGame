using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Network.NetworkObjects
{
    public class CanvasNetwork : MonoBehaviour
    {
        [FormerlySerializedAs("playersUI")] [SerializeField]
        private RectTransform _playersUI;

        [FormerlySerializedAs("readyButton")] [SerializeField]
        private Button _readyButton;

        [SerializeField] private CoreView _coreView;
        [SerializeField] private TMP_Text _connectionCount;
        [SerializeField] private Slider _loaderSlider;

        [SerializeField] private float _loadTimer;
        // [SerializeField] private ExitHost _exitHost;

        private QuizNetworkManager _networkManager;
        private int _connections;
        private bool _exit;

        public event Action StartQuiz;

        private void Start()
        {
            // _exitHost.Exit += OnExit;
            _playersUI.gameObject.SetActive(false);
            _loaderSlider.gameObject.SetActive(false);
            _readyButton.interactable = false;
            _readyButton.onClick.AddListener(ClickedReadyButton);
            _coreView.OnStart += ShowPlayersPanels;
            _networkManager = FindObjectOfType<QuizNetworkManager>();
            if (_networkManager == null)
            {
                Debug.LogError("QuizNetworkManager not found in the scene.");
                return;
            }

            _networkManager.OnClientDisconnected += HandleClientDisconnected;
            _networkManager.OnClientConnected += HandleClientConnected;
        }

        private void OnDestroy()
        {
            if (_networkManager != null)
            {
                _networkManager.OnClientConnected -= HandleClientConnected;
                _networkManager.OnClientDisconnected -= HandleClientDisconnected;
            }
        }

        public void HandleClientReady()
        {
            _readyButton.interactable = true;
        }

        public RectTransform GetPlayersPanel() => _playersUI;

        public void AddPlayer()
        {
            if (_connections < _networkManager.maxConnections)
            {
                _connections++;
                _connectionCount.text = _connections.ToString();
            }
        }

        private void ClickedReadyButton()
        {
            StartQuiz?.Invoke();
            // _playersPanel.SetActive(true);
            _readyButton.gameObject.SetActive(false);
            // StartCoroutine(ShowPlayersConnected());
        }

        private void OnExit()
        {
            _coreView.OnStart -= ShowPlayersPanels;
            //  _exitHost.Exit -= OnExit;
            DOTween.Kill(_loaderSlider);
        }

        private void ShowPlayersPanels()
        {
            _coreView.OnStart -= ShowPlayersPanels;
            _readyButton.gameObject.SetActive(false);
            _connectionCount.gameObject.transform.parent.gameObject.SetActive(false);
            _playersUI.gameObject.SetActive(true);
            _loaderSlider.gameObject.SetActive(true);
            _loaderSlider.value = 0;
            DOTween.Kill(_loaderSlider);
            _loaderSlider.DOValue(1, _loadTimer)
                .OnComplete(() =>
                {
                    if (_playersUI == null)
                        return;

                    _playersUI.gameObject.SetActive(false);
                    _loaderSlider.gameObject.SetActive(false);
                    _coreView.StartQuiz();
                });
        }

        private void HandleClientDisconnected()
        {
            _readyButton.interactable = false;
        }

        private void HandleClientConnected()
        {
            Debug.Log("Connected CLIENT CANVAS NETWORL");
            // readyButton.interactable = true;
        }

        public void PlayerOut()
        {
            _connections--;
            _connectionCount.text = _connections.ToString();
        }
    }
}