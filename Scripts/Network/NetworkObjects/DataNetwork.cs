using System.Collections.Generic;
using System.Linq;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.Installers.Settings.Quiz;
using Infrastructure.UIPanels.Boosters;
using Mirror;
using UnityEngine;

namespace Network.NetworkObjects
{
    public class DataNetwork : NetworkBehaviour
    {
        [SyncVar(hook = nameof(OnQuestionsSettingsChanged))]
        public QuestionsSettings _questionsSettings;

        [SyncVar(hook = nameof(OnPlayerSettingsChanged))]
        public PlayerSettings _playerSettings;

        [SerializeField] public List<PlayersAnswered> PlayersAnsweredsList;
        private CoreView _coreView;
        private int _answeredPlayers;
        public int _currentQuestionIndex;
        private bool _isLouping;
        private bool _botIsOn;
        private QuizHandler _quizHandler;
        private bool _answerBot;

        private const int MaxChunkSize = Constants.MAX_CHUNKS_SIZE;

        public void Initialize(QuestionsSettings questionsSettings, PlayerSettings playerSettings,
            QuizHandler quizHandler)
        {
            _questionsSettings = questionsSettings;
            _playerSettings = playerSettings;
            _quizHandler = quizHandler;
            _currentQuestionIndex = 0;
            PlayersAnsweredsList = new List<PlayersAnswered>();
        }

        [Server]
        public void SetQuestionsList(QuestionsSettings questionsSettings, PlayerSettings playerSettings, bool botIsOn)
        {
            _questionsSettings = questionsSettings;
            _playerSettings = playerSettings;
            _botIsOn = botIsOn;
            _questionsSettings.SetQuestionsList();
            ResetAnswers();
            if (_botIsOn)
                CreateBotsChoose();

            //RpcSetQuestions(_questionsSettings.Serialize(), _playerSettings.Serialize());
            string serializedQuestions = _questionsSettings.Serialize();
            var questionParts = NetworkUtils.SplitString(serializedQuestions, MaxChunkSize);

            for (int i = 0; i < questionParts.Count; i++)
                RpcSetQuestionsChunk(questionParts[i], i, questionParts.Count);


            string serializedPlayerSettings = _playerSettings.Serialize();
            RpcSetPlayers(serializedPlayerSettings);
        }

        private void CreateBotsChoose()
        {
            float maxCoins = PlayersAnsweredsList.FindAll(x => !x.IsBot).Max(x => x.Coins);
            foreach (PlayersAnswered playersAnswered in PlayersAnsweredsList)
            {
                if (playersAnswered.IsBot && playersAnswered.AnswerIcon == ' ')
                {
                    playersAnswered.AnswerIcon = _quizHandler.BotChoose(playersAnswered,
                        _questionsSettings.Questions.Questions[_currentQuestionIndex], maxCoins);
                }
            }
        }

        [Server]
        public void Clear()
        {
            PlayersAnsweredsList.Clear();
            //  _playerSettings.Clear();
        }

        private void ResetAnswers()
        {
            _currentQuestionIndex = 0;
            _answeredPlayers = 0;
            CreatePlayersAnsweredList();
        }

        private void CreatePlayersAnsweredList()
        {
            if (PlayersAnsweredsList == null)
            {
                PlayersAnsweredsList = new List<PlayersAnswered>();
            }

            PlayersAnsweredsList.Clear();
            PlayersAnsweredsList.RemoveRange(0, PlayersAnsweredsList.Count);
            foreach (var players in _playerSettings.PlayersNames)
            {
                if (!PlayersAnsweredsList.Exists(x => x.Name == players.Name))
                {
                    PlayersAnsweredsList.Add(new PlayersAnswered()
                    {
                        Name = players.Name, Coins = 0,
                        LoseStreak = 0, AvatarName = players.AvatarName,
                        IsBot = players.IsBot,
                        WinLevel = players.WinLevel,
                        AnswerIcon = ' ',
                        RightAnswers = 0
                    });
                }
            }
        }

        [ClientRpc]
        private void RpcSetQuestionsChunk(string chunk, int index, int totalChunks)
        {
            if (index == 0)
                _questionsSettings = new QuestionsSettings();

            _questionsSettings.AddChunk(chunk, index, totalChunks);
        }

        [ClientRpc]
        private void RpcSetPlayers(string serialize)
        {
            _playerSettings = PlayerSettings.Deserialize(serialize);

            CreatePlayersAnsweredList();

            if (_questionsSettings != null)
            {
                Debug.Log("Players updated on client");
                _coreView = GameObject.Find(Constants.CORE_CANVAS_PANEL).GetComponent<CoreView>();
                if (_coreView != null)
                {
                    _coreView.StartGame(_questionsSettings, _playerSettings, PlayersAnsweredsList);
                    _coreView.OnMakeAnswer += OnMakeAnswer;
                    _coreView.OnAttackEnemy += OnAttackEnemy;
                    _coreView.GetEnemiesAnswers += OnGetEnemiesAnswers;
                    _coreView.PlayerChoose += OnPlayerChoose;
                    _coreView.Reflect += OnPlayerReflect;
                    _coreView.ReflectAttacks += OnReflectAttacks;
                }
            }
        }

        private void OnReflectAttacks(string enemyName, string playerName, BoosterType type)
        {
            _coreView.ReflectAttacks -= OnReflectAttacks;
            CmdAttackEnemy(enemyName, playerName, type);
        }

        private void OnPlayerReflect(string playerName)
        {
            _coreView.Reflect -= OnPlayerReflect;
            CmdMirrorReflector(playerName);
        }

        [Command(requiresAuthority = false)]
        private void CmdMirrorReflector(string playerName, NetworkConnectionToClient sender = null)
        {
            PlayersAnsweredsList.Find(x => x.Name == playerName).IsMirror = true;
            RcpMirrorReflector(playerName);
        }

        [ClientRpc]
        private void RcpMirrorReflector(string playerName)
        {
            Debug.Log("Reflect DATANETWORK");

            _coreView.ReflectingPlayer(playerName);
        }

        #region PlayerChoose

        private void OnPlayerChoose(string playerName, char icon)
        {
            // _coreView.PlayerChoose -= OnPlayerChoose;
            CmdPlayerChoose(playerName, icon);
        }

        [Command(requiresAuthority = false)]
        private void CmdPlayerChoose(string playerName, char icon, NetworkConnectionToClient sender = null)
        {
            Debug.Log("CmdPlayerChoose DataaNetw");
            if (PlayersAnsweredsList.Exists(x => x.Name == playerName))
            {
                PlayersAnsweredsList.Find(x => x.Name == playerName).AnswerIcon = icon;
                RcpPlayerChoose(PlayersAnsweredsList);
            }
        }

        [ClientRpc]
        private void RcpPlayerChoose(List<PlayersAnswered> playersAnsweredsList)
        {
            //  _coreView.PlayerChoose += OnPlayerChoose;
            if (!_isLouping)
                return;
            if (_coreView != null)
                _coreView.UpdateEnemiesAnswers(playersAnsweredsList);
        }

        #endregion

        #region GetEnemiesAnswer

        private void OnGetEnemiesAnswers(string playerName)
        {
            _isLouping = true;
            CmdEnemiesAnswer(playerName);
        }

        [Command(requiresAuthority = false)]
        private void CmdEnemiesAnswer(string playerName, NetworkConnectionToClient sender = null)
        {
            Debug.Log("Cmdget enemies Answers DataaNetw");
            RcpGetEnemiesAnswers(playerName, PlayersAnsweredsList);
        }

        [ClientRpc]
        private void RcpGetEnemiesAnswers(string playerName, List<PlayersAnswered> playersAnsweredsList)
        {
            if (_coreView != null)
                _coreView.ShowEnemiesAnswers(playerName, playersAnsweredsList);
        }

        #endregion

        #region AttackEnemy

        private void OnAttackEnemy(string enemyName, string playerName, BoosterType type)
        {
            //  _coreView.OnAttackEnemy -= OnAttackEnemy;
            CmdAttackEnemy(enemyName, playerName, type);
        }

        [Command(requiresAuthority = false)]
        private void CmdAttackEnemy(string enemyName, string playerName, BoosterType type,
            NetworkConnectionToClient sender = null)
        {
            Debug.Log("CmdAttackEnemy DataaNetw");
            if (PlayersAnsweredsList.Exists(x => x.Name == enemyName))
            {
                switch (type)
                {
                    case BoosterType.Freeze:
                        PlayersAnsweredsList.Find(x => x.Name == enemyName).IsAttackedFreeze = true;
                        break;
                    case BoosterType.VocabularyBomb:
                        PlayersAnsweredsList.Find(x => x.Name == enemyName).IsAttackedBomb = true;
                        break;
                    case BoosterType.Rotate:
                        PlayersAnsweredsList.Find(x => x.Name == enemyName).IsAttackedRotate = true;
                        break;
                    case BoosterType.Comic:
                        PlayersAnsweredsList.Find(x => x.Name == enemyName).IsAttackComic = true;
                        break;
                }
                RcpAttackEnemy(enemyName, playerName, type);
            }
        }

        [ClientRpc]
        private void RcpAttackEnemy(string enemyName, string player, BoosterType type)
        {
            if (_coreView != null)
                _coreView.AttackEnemy(enemyName, player, type);
        }

        #endregion

        #region MakeAnswer

        private void OnMakeAnswer(Players player)
        {
            _coreView.PlayerChoose -= OnPlayerChoose;
            CmdReceiveAnswer(player.Name, player.Coins, player.LoseStreak, player.WinLevel);
        }

        [Command(requiresAuthority = false)]
        private void CmdReceiveAnswer(string playerName, float coins, float loseStreak, int winLevel,
            NetworkConnectionToClient sender = null)
        {
            Debug.Log("CmdReceiveAnswer DataaNetw");
            if (_botIsOn && !_answerBot)
            {
                _answerBot = true;
                CountBotsCoins();
            }


            var player = PlayersAnsweredsList.Find(x => x.Name == playerName);
            player.Coins = coins;
            player.LoseStreak = loseStreak;
            player.AnswerIcon = ' ';
            player.WinLevel = winLevel;
            player.IsAttackedBomb = false;
            player.IsAttackedFreeze = false;
            player.IsAttackedRotate = false;
            player.IsAttackComic = false;
            //  player.IsMirror = false;
            _answeredPlayers++;


            if (_answeredPlayers >= PlayersAnsweredsList.Count)
            {
                _answeredPlayers = 0;
                _answerBot = false;
                _currentQuestionIndex++;
                if (_currentQuestionIndex >= _playerSettings.BotAttackStartLevel)
                    BotAttackPlayers();

                CreateBotsChoose();
                PlayersAnsweredsList.ForEach(x => x.IsMirror = false);
                RpcNextQuestion(PlayersAnsweredsList);
            }
        }

        private void BotAttackPlayers()
        {
            PlayersAnswered[] players =
                PlayersAnsweredsList.FindAll(x => !x.IsBot).OrderByDescending(x => x.Coins).ToArray();
            PlayersAnswered leaderPlayer = players[0];
            foreach (var bot in PlayersAnsweredsList)
            {
                if (bot.IsBot && !bot.MakeAttacked)
                {
                    if ((bot.Coins < leaderPlayer.Coins)
                        && (!leaderPlayer.IsAttackedFreeze || !leaderPlayer.IsAttackedBomb))
                    {
                        var rnd = Random.Range(0, 100);
                        if (rnd <= _playerSettings.BotAttackChance)
                        {
                            if (!leaderPlayer.IsAttackedFreeze && !bot.IsAttackedFreeze)
                            {
                                MakeAttack(bot, leaderPlayer, BoosterType.Freeze, ref bot.IsAttackedFreeze,
                                    ref leaderPlayer.IsAttackedFreeze);
                            }
                            else
                            {
                                if (!leaderPlayer.IsAttackedBomb && !bot.IsAttackedBomb)
                                {
                                    MakeAttack(bot, leaderPlayer, BoosterType.VocabularyBomb, ref bot.IsAttackedBomb,
                                        ref leaderPlayer.IsAttackedBomb);
                                }
                                else
                                {
                                    if (!leaderPlayer.IsAttackedRotate && !bot.IsAttackedRotate)
                                    {
                                        MakeAttack(bot, leaderPlayer, BoosterType.Rotate, ref bot.IsAttackedRotate,
                                            ref leaderPlayer.IsAttackedRotate);
                                    }
                                    else
                                    {
                                        if (!leaderPlayer.IsAttackComic && !bot.IsAttackComic)
                                        {
                                            MakeAttack(bot, leaderPlayer, BoosterType.Comic, ref bot.IsAttackComic,
                                                ref leaderPlayer.IsAttackComic);
                                        }
                                    }
                                }
                            }

                            if (bot.IsAttackedFreeze && bot.IsAttackedBomb && bot.IsAttackedRotate && bot.IsAttackComic)
                                bot.MakeAttacked = true;
                        }
                    }
                }
            }
        }

        private void MakeAttack(PlayersAnswered bot, PlayersAnswered leaderPlayer, BoosterType type, ref bool attackBot,
            ref bool attackleader)
        {
            attackBot = true;
            attackleader = true;
            if (!leaderPlayer.IsMirror)
                RcpAttackEnemy(leaderPlayer.Name, bot.Name, type);
            else
            {
                Debug.Log(leaderPlayer + " IS REFLECT");
            }
        }

        [ClientRpc]
        private void RpcNextQuestion(List<PlayersAnswered> playersAnswereds)
        {
            Debug.Log("RPCNextQuest DataaNetw");
            _isLouping = false;
            PlayersAnsweredsList = playersAnswereds;
            _currentQuestionIndex++;

            if (_currentQuestionIndex < _questionsSettings.Questions.Questions.Count
                && _currentQuestionIndex % _playerSettings.QuestionAmount != 0)
            {
                if (_coreView != null)
                {
                    _coreView.PlayerChoose += OnPlayerChoose;
                    _coreView.ShowNextQuestion(PlayersAnsweredsList);
                }
            }
            else
            {
                QuizComplete();
            }
        }

        #endregion

        private void CountBotsCoins()
        {
            float maxCoins = PlayersAnsweredsList.FindAll(x => !x.IsBot).Max(x => x.Coins);

            foreach (var playersAnswered in PlayersAnsweredsList)
            {
                if (playersAnswered.IsBot)
                {
                    if (playersAnswered.AnswerIcon == ' ')
                    {
                        playersAnswered.AnswerIcon = _quizHandler.BotChoose(playersAnswered,
                            _questionsSettings.Questions.Questions[_currentQuestionIndex], maxCoins);
                    }

                    _answeredPlayers++;
                    float speed = _playerSettings.BotStartSpeed;
                    _quizHandler.BotAnswer(playersAnswered,
                        _questionsSettings.Questions.Questions[_currentQuestionIndex], speed, maxCoins);
                    playersAnswered.AnswerIcon = ' ';
                }
            }
        }

        private void QuizComplete()
        {
            PlayersAnsweredsList.ForEach(x => x.Total = x.Coins);
            float maxValue = PlayersAnsweredsList.Max(x => x.Total);

            PlayersAnswered winner = PlayersAnsweredsList.Find(x => x.Total == maxValue);
            _coreView.OnMakeAnswer -= OnMakeAnswer;
            _coreView.PlayerChoose -= OnPlayerChoose;

            _coreView.CompleteQuiz(winner.Name, PlayersAnsweredsList.OrderByDescending(x => x.Coins).ToList());
            PlayersAnsweredsList.Clear();
            //  _playerSettings.Clear();
            //_playersAnswered.Clear();
        }

        private void OnQuestionsSettingsChanged(QuestionsSettings oldSettings, QuestionsSettings newSettings)
        {
            _questionsSettings = newSettings;
        }

        private void OnPlayerSettingsChanged(PlayerSettings oldSettings, PlayerSettings newSettings)
        {
            _playerSettings = newSettings;
        }
    }
}