using System.Collections;
using Building;
using GoogleSheets;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Bots;
using Infrastructure.Installers.Settings.Build;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.Installers.Settings.Network;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.Installers.Settings.Quiz;
using Infrastructure.Level;
using Network;
using UnityEngine;
using Zenject;

namespace Infrastructure
{
    public class Bootstrap: MonoBehaviour, ICoroutineRunner
    {
        [SerializeField] private GoogleSheetsReader googleSheetsReader;
        private IEventBus _eventBus;
        private LevelLoader _levelLoader;
        private QuestionsSettings _questionsSettings;
        private NetworkAddress _networkAdress;
        private BuildsService _buildsService;
        private BotsConfig _botsConfig;
        private ShopsService _shopsService;
        private PlayerSettings _playersSettings;
        private InventoryService _inventoryService;
        private EnergyService _energyService;
        private IMainPlayerLoader _mainPlayerLoader;

        [Inject]
        public void Construct(IEventBus eventBus, LevelLoader levelLoader, PlayerSettings playerSettings,
            QuestionsSettings questionsSettings, NetworkAddress networkAddress,
            BotsConfig botsConfig, BuildsService buildsService, ShopsService shopsService, InventoryService inventoryService
            ,EnergyService energyService, IMainPlayerLoader mainPlayerLoader)
        {
            _eventBus = eventBus;
            _levelLoader = levelLoader;
            _playersSettings = playerSettings;
            _questionsSettings = questionsSettings;
            _networkAdress = networkAddress;
            _botsConfig = botsConfig;
            _buildsService = buildsService;
            _shopsService = shopsService;
            _inventoryService = inventoryService;
            _energyService = energyService;
            _mainPlayerLoader = mainPlayerLoader;
        }

        private void Awake()
        {
          //  _eventBus.Subscribe<LevelWin>(OnLevelWin);
          //  _eventBus.Subscribe<LevelLost>(OnLevelLost);
          Application.targetFrameRate = 60;
            googleSheetsReader.OnComlete += OnCompleteSpreadSheets;
     
            googleSheetsReader.ReadDataAnyway();
        
             _buildsService.LoadSaves(); 
          _inventoryService.Initialize(_eventBus, _shopsService.BoosterSettings);
           DontDestroyOnLoad(this);
        }


        private void OnCompleteSpreadSheets()
        {
            _questionsSettings.FilePath=Resources.Load<TextAsset>(Constants.GAME_DTO);
            _botsConfig.SetBotsConfigs();
            _shopsService.SetShopConfigs();
            _playersSettings.SetCoreConfigs();
         //   _energyService = new EnergyService();
            _energyService.SetEnergiConfig(_eventBus, _mainPlayerLoader, this);
         //   _playersSettings.SetBoosters(_shopsConfig.ShopSettings);
          //  _playersSettings.InitEvents(_eventBus);
            _eventBus.Invoke(new LoadNetwork());
        }


        private void OnLevelLost(LevelLost obj)
        {
          //  looseEffect.Play();
        }

        private void OnLevelWin(LevelWin obj)
        {
          //  winEffect.Play();
        }

        public void OnStopCoroutine(IEnumerator coroutine)
        {
            
        }
    }
}