using System;
using System.Collections.Generic;
using GoogleSheets;
using Infrastructure.Installers.Settings.Bots;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.Installers.Settings.Network;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.Installers.Settings.Quiz;
using kcp2k;
using Mirror;
using Network.NetworkObjects;
using Network.NetworkObjects.Messages;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;
using Object = UnityEngine.Object;

namespace Network
{
    public class QuizNetworkManager : NetworkManager
    {
        [SerializeField] private bool _isLocal;

        [FormerlySerializedAs("minPlayers")] [SerializeField]
        private int _minPlayers = 2;

        [SerializeField] private float _timer;

        [FormerlySerializedAs("questlistPrefab")] [SerializeField]
        private GameObject _questlistPrefab;

        [SerializeField]private GameObject botPrefab;

        private static new QuizNetworkManager singleton { get; set; }
        private PlayerSettings _playerSettings;
        private CoreView _coreView;
        private QuestionsSettings _questionsSettings;
        private DataNetwork _dataNetwork;
        private GameObject _questionsNetworkManagerObject;
        private QuizHandler _quizHandler;
        private NetworkAddress _networkAddress;
        private bool _allIsReady;
        private MainPlayerLoader _mainPlayerLoader;
        private bool _timerIsOn;
        private bool _botIsOn;
      //  private BotHandler _botHandler;
        private BotsConfig _botsConfig;

        public event Action OnClientConnected;
        public event Action OnClientDisconnected;
        public event Action OnClientsReady;
        public static event Action<NetworkConnection> OnServerReadied;

        public List<PlayerHandler> Players { get; } = new List<PlayerHandler>();

        [Inject]
        public void Construct(PlayerSettings playerSettings, QuestionsSettings questionsSettings,
            QuizHandler quizHandler, NetworkAddress networkAddress,BotsConfig botsConfig, MainPlayerLoader mainPlayerLoader)
        {
            _playerSettings = playerSettings;
            _questionsSettings = questionsSettings;
            _quizHandler = quizHandler;
            _networkAddress = networkAddress;
            _botsConfig = botsConfig;
            _mainPlayerLoader = mainPlayerLoader;
        }

        public override void Awake()
        {
            base.Awake();
            singleton = this;
            Debug.Log("Awake networkManager");
            if (!_isLocal)
            {
                _networkAddress.SetNetworkAddress();
                networkAddress = _networkAddress.Address;

                var kcpTransport = GetComponent<KcpTransport>();
                if (kcpTransport != null && ushort.TryParse(_networkAddress.UdpPort, out ushort udpPort))
                {
                    kcpTransport.Port = udpPort;
                    Debug.Log($"KcpTransport Port set to {udpPort}");
                }
                else
                {
                    Debug.LogError("KcpTransport not found or invalid UdpPort");
                }
            }
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            _questionsNetworkManagerObject = Instantiate(_questlistPrefab);
            _dataNetwork = _questionsNetworkManagerObject.GetComponent<DataNetwork>();
            _dataNetwork.Initialize(_questionsSettings, _playerSettings, _quizHandler);

            NetworkServer.Spawn(_questionsNetworkManagerObject);

            NetworkServer.RegisterHandler<ReadyToStartMessage>(OnReadyToStartMessage);
            NetworkServer.RegisterHandler<ClientDisconnectMessage>(OnClientLeaveMessage);
            Debug.Log("StartServer");
            DontDestroyOnLoad(_questionsNetworkManagerObject);
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _playerSettings.Clear();
            PlayerHandler.Init(_playerSettings, _quizHandler, _mainPlayerLoader);
            Debug.Log("Start Client");
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            base.OnServerConnect(conn);
            Debug.Log("Server connected");
            if (_allIsReady)
            {
                Debug.Log("ALL IS READY");
                if (conn != null && conn.isReady)
                {
                    Debug.Log("GAME IS RUNNING Disconnect connection");
                    var player = conn.identity.GetComponent<PlayerHandler>();
                    PlayerAuth._playerNames.Remove(player.playerName);
                    conn.Disconnect();
                }
            }
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            Debug.Log("Client connect");
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            base.OnServerAddPlayer(conn);
            Debug.Log("'Server add player'");
            var player = conn.identity.GetComponent<PlayerHandler>();
            if (!Players.Contains(player))
            {
                Debug.Log("Player NOT exist");
                if (Players.Count >= maxConnections)
                {
                    Debug.Log("Player >= maxConnection");
                    foreach (PlayerHandler playerHandler in Players)
                    {
                        if (playerHandler.isbot)
                        {
                            Debug.Log("Player is bot remove");
                            Players.Remove(playerHandler);
                            playerHandler.Disconnect();
                            break;
                        }
                    }
                }
                Debug.Log("PlayerADD to list");
                Players.Add(player);
                _timerIsOn = true;
            }

            if (Players.Count >= _minPlayers)
            {
                OnActivateStartQuizButton();
            }
        }

        private void FixedUpdate()
        {
            if(!_timerIsOn)
                return;
            _timer -= Time.deltaTime;
            if(_timer<=0)
            {
                _timerIsOn = false;
                var connections = maxConnections - Players.Count;
                for (int i = 0; i < connections; i++)
                {
                    CreateBots(i);
                }
                
            }
        }

        private void CreateBots(int i)
        {
            Debug.Log("Crerate BOT");
            var bot = Instantiate(playerPrefab);
           var botHandler = bot.GetComponent<PlayerHandler>();
           BotsSheets botConf=null;
           do
           { 
               botConf = _botsConfig.GetRandomName();
           } while (Players.Find(x => x.playerName == botConf.BotName));
            
            botHandler.playerName =botConf.BotName;
            botHandler.playerAvatarName=_playerSettings.Avatars[Int32.Parse(botConf.BotAvatar)];
            botHandler.isbot = true;
            NetworkServer.Spawn(bot);
            _botIsOn = true;
            Players.Add(botHandler);
            OnActivateStartQuizButton();
        }

        private void OnClientLeaveMessage(NetworkConnectionToClient conn, ClientDisconnectMessage mesg)
        {
        }

        private void OnReadyToStartMessage(NetworkConnection conn, ReadyToStartMessage msg)
        {
            Debug.Log("Received ReadyToStartMessage from CLIENT");
            if (!NotReadyPlayer())
            {
                _allIsReady = true;
                var auth = GetComponent<PlayerAuth>();
                auth.OnGame = true;
                StartGame();
            }
        }

        private void OnActivateStartQuizButton()
        {
            foreach (var player in Players)
            {
                player.RpcOnActivateStartQuizButton();
            }
        }

        private bool NotReadyPlayer()
        {
            return Players.Exists(x => !x.IsReady);
        }

        [Server]
        private void StartGame()
        {
            // await Task.Delay(10000);
            Debug.Log("StartGameInManager");
            _playerSettings.Clear();
            maxConnections = Players.Count;
            if (_dataNetwork != null)
            {
                if (Players.Count > 0)
                {
                    foreach (var player in Players)
                    {
                        _playerSettings.AddNewPlayer(player);
                    }

                   /* if (_botIsOn)
                    {
                        _playerSettings.AddNewBot(_botHandler);
                    }*/
                    //  _dataNetwork.SetPlayer(_playerSettings);
                }

                _dataNetwork.SetQuestionsList(_questionsSettings, _playerSettings, _botIsOn);
            }
        }

        public override void OnClientDisconnect()
        {
            base.OnClientDisconnect();
            Debug.Log("Disconnect Player");
            ClearAllData();
        }


        [Server]
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            Debug.Log("My ServerDisconnect");
            if (conn.identity != null && conn.identity.TryGetComponent(out PlayerHandler player))
            {
                if (Players.Contains(player))
                {
                    Players.Remove(player);
                    Debug.Log("Delete Player");
                    PlayerAuth._playerNames.Remove(player.playerName);
                }

                DisconnectAllClients();
                if (numPlayers <= 0)
                {
                    Debug.Log("NumPlayer " + numPlayers);
                    // ClearAllData();
                }
            }

            base.OnServerDisconnect(conn);
        }

        [Server]
        private void DisconnectAllClients()
        {
            Debug.Log("Disconnecting all clients...");
            var playersCopy = new List<PlayerHandler>(Players);
            foreach (var player in playersCopy)
            {
                var connection = player.connectionToClient;
                PlayerAuth._playerNames.Remove(player.playerName);
                if (connection != null && connection.isReady)
                {
                    connection.Disconnect();
                }
               
                if (_botIsOn)
                {
                    player.Disconnect();
                }
                
            }

            ClearAllData();
        }

        private void ClearAllData()
        {
            Debug.Log("Clear All Data Command");
            Players.Clear();
            maxConnections = 4;
            //_playerSettings.Clear();
            _allIsReady = false;
            _botIsOn = false;
            _timer = 5;
            if (_dataNetwork != null)
            {
                _dataNetwork.Clear();
            }

            var auth = GetComponent<PlayerAuth>();
            auth.OnGame = false;
        }

        public override void OnStopServer()
        {
            base.OnStopServer();
            Debug.Log("StopServer");
            ClearAllData();
        }
    }
}