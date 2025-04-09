using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.Installers.Settings.Players;
using Mirror;
using Network.NetworkObjects.Messages;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Network.NetworkObjects
{
    public class PlayerHandler : NetworkBehaviour
    {
        [Header("Player UI")] [SerializeField] private GameObject playerUIPrefab;
        private static readonly List<PlayerHandler> playersList = new List<PlayerHandler>();
        private GameObject playerUIObject;
        private PlayerView playerUI = null;
        private CanvasNetwork _canvasNetwork;
        private static PlayerSettings _playerSettings;

        public event Action<string> OnPlayerStatusChanged;
        public event Action<string> OnPlayerNameChanged;

        public event Action<ushort> OnPlayerDataChanged;
        public event Action<float> OnPlayerCoinsChanged;
        public event Action<string> OnPlayerAvatarChanged;


        #region SyncVars

        [Header("SyncVars")] [SyncVar(hook = nameof(PlayerStatusChanged))]
        public string playerStatus = "ready";

        [SyncVar(hook = nameof(PlayerNameChanged))]
        public string playerName = "None";


        [SyncVar(hook = nameof(PlayerDataChanged))]
        public ushort playerData = 0;

        [SyncVar(hook = nameof(PlayerCoinsChanged))]
        public float playerCoins = 0;

        [SyncVar(hook = nameof(PlayerAvatarChanged))]
        public string playerAvatarName;

        public string playerAvatar;

        #endregion

        private QuizNetworkManager _manager;
        private CoreView _coreView;
        private static QuizHandler _quizHandler;
        private bool _isReady = true;
        private static int _playerAvatarIndex;
        public bool isbot;

        private QuizNetworkManager Manager
        {
            get
            {
                if (_manager != null)
                    return _manager;

                return _manager = NetworkManager.singleton as QuizNetworkManager;
            }
        }

        public bool IsReady => _isReady;
        public string Avatar => _playerSettings.Avatars[_playerAvatarIndex];

        //servers

        public static void Init(PlayerSettings playerSettings,
            QuizHandler quizHandler, MainPlayerLoader playerLoader)
        {
            Debug.Log("init plHandler");
            _playerSettings = playerSettings;
            _quizHandler = quizHandler;
            _playerAvatarIndex = playerLoader.PlayerIndexAvatar;
            Debug.Log("PLAYER AVATAR " + _playerAvatarIndex);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            playersList.Add(this);
            if(connectionToClient!=null)
                playerName = (string)connectionToClient.authenticationData;
            playerData = (ushort)Random.Range(100, 1000);
            playerAvatar = LoadSpriteByName(playerAvatarName);
        }

        public override void OnStopServer()
        {
            playersList.Remove(this);
        }


        [Command]
        private void CmdStartQuiz()
        {
            _isReady = true;
            playerStatus = "Ready";
            //  Manager.NotifyPlayersOfReadyState();
            RpcClientReady();
        }

        [TargetRpc]
        private void RpcClientReady()
        {
            if (NetworkClient.isConnected)
            {
                ReadyToStartMessage message = new ReadyToStartMessage
                {
                    IsReady = true
                };
                NetworkClient.Send(message);
            }
            else
            {
                Debug.Log("NOT CONNECTEDDD");
            }
        }

        //Clients
        [ClientRpc]
        public void RpcOnActivateStartQuizButton()
        {
            _canvasNetwork.HandleClientReady();
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            _playerSettings.CurrentPlayer = playerName;
            // playerAvatar = _playerAvatarIndex;
            playerAvatarName = _playerSettings.Avatars[_playerAvatarIndex];
            Debug.Log("AuthPL");
        }

        public override void OnStartClient()
        {
            Debug.Log("plStartClient");

            _canvasNetwork = GameObject.Find(Constants.CANVAS_NETWORK).GetComponent<CanvasNetwork>();
            _canvasNetwork.AddPlayer();
            // playerAvatar = _playerAvatarIndex;
            if (!string.IsNullOrEmpty(playerAvatarName))
            {
                Debug.Log("SetPlayerAvatar");
                playerAvatar = LoadSpriteByName(playerAvatarName);
                OnPlayerAvatarChanged?.Invoke(playerAvatar);
            }

            CreatePlayerView();
        }

        private void CreatePlayerView()
        {
            playerUIObject = Instantiate(playerUIPrefab, _canvasNetwork.GetPlayersPanel());
            playerUI = playerUIObject.GetComponent<PlayerView>();
            _playerSettings.CurrentPlayer = playerName;

            OnPlayerStatusChanged = playerUI.OnPlayerNumberChanged;
            OnPlayerNameChanged = playerUI.OnPlayerNameChanged;
            OnPlayerCoinsChanged = playerUI.OnPlayerCoinsChanged;
            OnPlayerAvatarChanged = playerUI.OnPlayerAvatarChanged;

            InvokeChangesOnPlayerView();
        }

        public override void OnStartLocalPlayer()
        {
            Debug.Log("Start Local Player");
            _canvasNetwork.gameObject.SetActive(true);
            _canvasNetwork.StartQuiz += CmdStartQuiz;
          //  _quizHandler.SetCurrentPlayer(playerName);
            var coreView = GameObject.Find(Constants.CORE_CANVAS_PANEL).GetComponent<CoreView>();
            coreView.SetCurrentPlayer(playerName);
            _quizHandler.PlayerTakeCoins += AddCoins;

            InvokeChangesOnPlayerView();
        }

        private void InvokeChangesOnPlayerView()
        {
            OnPlayerStatusChanged?.Invoke(playerStatus);
            OnPlayerNameChanged?.Invoke(playerName);
            OnPlayerCoinsChanged?.Invoke(playerCoins);
            OnPlayerAvatarChanged?.Invoke(playerAvatarName);
        }

        public override void OnStopLocalPlayer()
        {
            _canvasNetwork.gameObject.SetActive(false);
        }

        public override void OnStopClient()
        {
            Manager.Players.Remove(this);
            OnPlayerStatusChanged = null;
            OnPlayerNameChanged = null;
            OnPlayerDataChanged = null;
            OnPlayerCoinsChanged = null;
            OnPlayerAvatarChanged = null;

            playerCoins = 0;
            Destroy(playerUIObject);
        }

        //Local

        private void AddCoins(float coins)
        {
            playerCoins = coins;
        }


        private void PlayerStatusChanged(string _, string newPlayerStatus)
        {
            playerStatus = newPlayerStatus;
            OnPlayerStatusChanged?.Invoke(newPlayerStatus);
        }

        private void PlayerNameChanged(string _, string newPlayerName)
        {
            OnPlayerNameChanged?.Invoke(newPlayerName);
        }

        private void PlayerDataChanged(ushort _, ushort newPlayerData)
        {
            OnPlayerDataChanged?.Invoke(newPlayerData);
        }

        private void PlayerCoinsChanged(float _, float newPlayerCoins)
        {
            OnPlayerCoinsChanged?.Invoke(newPlayerCoins);
        }

        private void PlayerAvatarChanged(string _, string newPlayerAvatarName)
        {
            playerAvatarName = newPlayerAvatarName;
            playerAvatar = LoadSpriteByName(playerAvatarName);
            OnPlayerAvatarChanged?.Invoke(playerAvatarName);
        }

        private string LoadSpriteByName(string spriteName)
        {
           // return Resources.Load<Sprite>(spriteName);
            return spriteName;
        }


        public void Disconnect()
        {
            Destroy(gameObject);
        }
    }
}