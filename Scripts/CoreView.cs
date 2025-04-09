using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.Installers.Settings.Quiz;
using Infrastructure.UIPanels;
using Infrastructure.UIPanels.Boosters;
using Infrastructure.UIPanels.FinishPanels;
using Mirror;
using Network.NetworkObjects;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class CoreView : MonoBehaviour
{
    [Header("Panels")] [FormerlySerializedAs("gameplayPanel")] [SerializeField]
    private QuizPanel _quizPanel;

    [SerializeField] private WinPanel winPanel;
    [SerializeField] private LoosePanel loosePanel;
    [FormerlySerializedAs("_freezePanel")] [SerializeField] private TutorialPanel tutorialPanel;
    
    private List<PlayersAnswered> _playersAnswereds;

    private IEventBus _eventBus;
    private QuestionsSettings _questSettingz;
    private IQuizHandler _quizHandler;
    public PlayerSettings _playerSettings;
    private char _icon;
    public Players _currentPlayer;
    private string _playerName;
    private bool _reflector;
    private string _winner;
    private AnimationSettings _animationSettings;
    private string _enemyName;

    public static event Action<List<PlayersAnswered>> OnStartingGame;
    public event Action OnStart;
    public event Action<Players> OnMakeAnswer;
    public event Action<string, string, BoosterType> OnAttackEnemy;
    public event Action<string> GetEnemiesAnswers;
    public event Action<string, char> PlayerChoose;
    public event Action<string, string, BoosterType> ReflectAttacks;
    public event Action<string> Reflect;

    [Inject]
    public void Construct(IEventBus eventBus, IQuizHandler quizHandler,
        QuestionsSettings questionsSettings, AnimationSettings animationSettings)
    {
        _eventBus = eventBus;
        _quizHandler = quizHandler;
        // _playerSettings = playerSettings;
        _questSettingz = questionsSettings;
        _animationSettings = animationSettings;
    }

    private void Start()
    {
        _quizPanel.ClickedPanel += OnPauseGame;
        _quizPanel.CompleteQuiz += OnCompleteQuiz;
        loosePanel.ClickedPanel += RestartGame;
        winPanel.ClickedPanel += RestartGame;
        //  _exitButton.onClick.AddListener(RestartGame);
        HideAllPanels();
        OnStartingGame += OnPlayGame;
    }

    private void OnDestroy()
    {
        loosePanel.ClickedPanel -= RestartGame;
        _quizPanel.ClickedPanel -= OnPauseGame;
        _quizPanel.CompleteQuiz -= OnCompleteQuiz;
        winPanel.ClickedPanel -= RestartGame;
    }

    public void SetCurrentPlayer(string playerName)
    {
        _playerName = playerName;
    }

    public void StartGame(QuestionsSettings questionsSettings, PlayerSettings playerSettings,
        List<PlayersAnswered> playersAnswereds)
    {
        _playersAnswereds = playersAnswereds;
        _questSettingz = questionsSettings;
        _playerSettings = playerSettings;
        _currentPlayer = _playerSettings.PlayersNames.Find(x => x.Name == _playerName);

        _quizHandler.Init(_playerSettings);
        _quizHandler.WinPlayer += PlayerWin;
        _quizHandler.LoosePlayer += PlayerLoose;
        _quizPanel.PlayerMakeAnswer += OnPlayerAnswer;
        _quizPanel.PlayerChoose += OnPlayerChoose;
        _quizPanel.PlayerBanque += OnPlayerBanque;
        _quizPanel.PlayerChooseEnemyToAttack += OnChooseEnemyToAttack;
        _quizPanel.DisplayAllEnemiesAnswers += OnDisplayAllEnemiesAnswers;
        _quizPanel.MakeFreeze += MakeFreezePanel;
        _quizPanel.ReflectAttack += ReflectAllAttacks;
        //Show plyersUiPanel on Canvas
        //=>wait 10 sec =>  OnStartingGame?.Invoke(_playersAnswereds);
        //  OnStartingGame?.Invoke(_playersAnswereds);
        OnStart?.Invoke();
    }

    private void ReflectAllAttacks()
    {
        _reflector = true;
        Reflect?.Invoke(_playerName);
    }

    public void StartQuiz()
    {
        OnStartingGame?.Invoke(_playersAnswereds);
    }

    public void ShowEnemiesAnswers(string playerName, List<PlayersAnswered> playersAnsweredsList)
    {
        if (_currentPlayer.Name == playerName)
        {
            _quizPanel.ShowEnemiesAnswers(playersAnsweredsList);
        }
    }

    public void UpdateEnemiesAnswers(List<PlayersAnswered> playersAnsweredsList)
    {
        _quizPanel.ShowEnemiesAnswers(playersAnsweredsList);
    }

    public void ShowNextQuestion(List<PlayersAnswered> playersAnswereds)
    {
        if (_reflector)
        {
            _reflector = false;
            Debug.Log("Clear reflector");
        }
        _playersAnswereds.Clear();
        _playersAnswereds = playersAnswereds;
        foreach (var player in _playersAnswereds)
        {
            _playerSettings.PlayersNames.Find(x => x.Name == player.Name).Coins = player.Coins;
        }

        _quizPanel.DisplayRightAnswer(_playersAnswereds);
    }

    public void AttackEnemy(string enemyName, string player, BoosterType type)
    {
        if (_currentPlayer.Name == enemyName)
        {
            if (!_reflector)
            {
                Debug.Log("Reflect false");

                _quizPanel.Attacked(type);
                _quizPanel.ShowPlayersAttacks(enemyName, type);
            }
            else
            {
                Debug.Log("Reflect true");
                ReflectAttacks?.Invoke(player,_playerName,type);
            }
        }
        else
        {
            if (player == _enemyName)
            {
                Debug.Log("NOT CURRENT SHOW_PL_ATTACk");
                Debug.Log("BUT ENEMY==");
            }
            else
            {
                Debug.Log("NOT CURRENT SHOW_PL_ATTACk");
                _quizPanel.ShowPlayersAttacks(enemyName, type);
            }
          
        }
        
       
        
    }

    public async Task CompleteQuiz(string winnerName, List<PlayersAnswered> playersAnswereds)
    {
        _quizPanel.PlayerMakeAnswer -= OnPlayerAnswer;
        _playersAnswereds = playersAnswereds;
        _quizPanel.Finish(_playersAnswereds); 
        _quizHandler.CompleteQuiz(_playersAnswereds, _currentPlayer);
        await Task.Delay(1000);
        if (_currentPlayer.Name != winnerName)
        {
            PlayerLoose(_currentPlayer);
        }
        else
        {
            PlayerWin(_currentPlayer);
        }
    }

    public void ReflectingPlayer(string playerName)
    {
        _quizPanel.ReflectingPlayer(playerName);
    }

    private void OnPlayerChoose(char icon)
    {
        PlayerChoose?.Invoke(_currentPlayer.Name, icon);
    }

    private void OnDisplayAllEnemiesAnswers()
    {
        // _quizPanel.DisplayAllEnemiesAnswers -= OnDisplayAllEnemiesAnswers;
        GetEnemiesAnswers?.Invoke(_currentPlayer.Name);
    }

    private void MakeFreezePanel(float timer)
    {
        //_quizPanel.MakeFreeze -= MakeFreezePanel;
        tutorialPanel.FinishedFreeze += EndingFreeze;
        tutorialPanel.Show(timer);
    }

    private void EndingFreeze()
    {
        _quizPanel.EndingFreeze();
    }

    private void OnChooseEnemyToAttack(string enemyName, BoosterType type)
    {
        _enemyName = enemyName;
        OnAttackEnemy?.Invoke(enemyName, _playerName, type);
    }

    private void OnPlayerBanque()
    {
        _quizPanel.PlayerBanque -= OnPlayerBanque;
        _quizHandler.PlayerVaBanque(_currentPlayer);
    }

    private void OnPlayerAnswer(bool isRight, float coins)
    {
        _quizHandler.PlayerAnswer(isRight, coins, _currentPlayer.Name);
        _quizHandler.FinishChoosingPlayer += ChoosePlayerCompleted;
    }

    private void PlayerLoose(Players player)
    {
        _quizHandler.LoosePlayer -= PlayerLoose;
        _quizHandler.WinPlayer -= PlayerWin;
        if (loosePanel == null)
            return;

        HideAllPanels();
        loosePanel.Initialize(_animationSettings.FinishPanelCoinsDuration, _animationSettings.FinishPanelCoinsRotateAngle, player, _playersAnswereds);
        loosePanel.Show();
    }

    private void PlayerWin(Players player)
    {
        _quizHandler.WinPlayer -= PlayerWin;
        _quizHandler.LoosePlayer -= PlayerLoose;

        if (winPanel == null)
            return;
        HideAllPanels();
        winPanel.Initialize(_animationSettings.FinishPanelCoinsDuration, _animationSettings.FinishPanelCoinsRotateAngle, player, _playersAnswereds);
        winPanel.Show();
    }

    private void ChoosePlayerCompleted(Players player)
    {
        _quizHandler.FinishChoosingPlayer -= ChoosePlayerCompleted;
        OnMakeAnswer?.Invoke(player);
    }

    private void OnCompleteQuiz()
    {
      
    }

    private void OnPauseGame()
    {
        _eventBus.Invoke(new PauseGame());
    }

    private void OnPlayGame(List<PlayersAnswered> playersAnswereds)
    {
        OnStartingGame -= OnPlayGame;
        _playersAnswereds = playersAnswereds;

        _eventBus.Invoke(new PlayGame());
        HideAllPanels();
        if (_quizPanel == null)
            return;

        _quizPanel.Initialize(_playersAnswereds,
            _currentPlayer.Name);

        _quizPanel.InitializeQuiz(_questSettingz.Questions, _playerSettings.QuestionAmount);
        _quizPanel.Show();
    }

    private void RestartGame()
    {
        ExitServer();
    }

    private void ExitServer()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }
    }

    private void HideAllPanels()
    {
        if (_quizPanel != null)
            _quizPanel.Hide();

        if (loosePanel != null)
            loosePanel.Hide();

        if (winPanel != null)
            winPanel.Hide();
    }
}