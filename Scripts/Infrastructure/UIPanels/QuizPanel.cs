using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.Installers.Settings.Quiz;
using Infrastructure.UIPanels.Answers;
using Infrastructure.UIPanels.Boosters;
using Infrastructure.UIPanels.PlayerPanel;
using Network.NetworkObjects;
using TMPro;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

namespace Infrastructure.UIPanels
{
    public class QuizPanel : PanelBase
    {
        [SerializeField] private TimerView _timerView;
        [Header("Quiz")] 
        [SerializeField] private QuestionView _question;
        [SerializeField] private TMP_Text _questionNumber;
        [SerializeField] private CoinsDisplay _coinsDisplay;
        [SerializeField] private AnswerView[] _answers;
        [SerializeField] private BoostersPanels _boostersPanels;
        [SerializeField] private PlayerUIPanel _playerUIPanel;
        [SerializeField] private AnswerHistoryPanel _answerHistoryPanel;
        [SerializeField] private TMP_FontAsset _normalTextStyle;
        [SerializeField] private TMP_FontAsset _comicSansFont;


        private List<Question> _quest;
        private int _index = 0;
        private bool _timeIsOut;
        private bool _isAnswered;
     
        private List<PlayersAnswered> _playersList;
      //  private float _maxTimeToAnswer;
        private bool _isLate;
        private string _currentPlayer;
        private BoosterType _boosterType;
        private float _restTime;
        private bool _isRight;
        private int _questionsAmount;
        private int _numQuestion;
        private bool _attackedFreeze;
        private bool _attackedBomb;
        private float _freezeTimer;
        private bool _attackedRotate;
        private bool _attackedComic;
        private char[] _answerIcon;
        private PlayerSettings _playerSettings;
        private bool _onHidingAnswer;
        private bool _changeTextStyle;
        private bool _usedAttackBooster;
        private AnimationSettings _animationSettings;
        private bool _mirror;

        public event Action CompleteQuiz;
        public event Action<bool, float> PlayerMakeAnswer;
        public override event Action ClickedPanel;
        public event Action PlayerBanque;
        public event Action<string, BoosterType> PlayerChooseEnemyToAttack;
        public event Action<float> MakeFreeze;
        public event Action DisplayAllEnemiesAnswers;
        public event Action<char> PlayerChoose;
        public event Action ReflectAttack;

        [Inject]
        public void Construct(PlayerSettings playerSettings,AnimationSettings animationSettings)
        {
            _playerSettings = playerSettings;
            _animationSettings = animationSettings;
        }

        protected override void OnClickedPanel()
        {
            ClickedPanel?.Invoke();
        }

        public void Initialize(List<PlayersAnswered> playersAnswereds, string currentPlayer)
        {
            _playersList = playersAnswereds;
            // _maxTimeToAnswer = _playerSettings.TimerToAnswer;
            _freezeTimer = _playerSettings.FreezeTimer;
            _currentPlayer = currentPlayer;
            _timerView.Initialize(_playerSettings.TimerToAnswer, _animationSettings.TimerClockScale, _animationSettings.TimerClockScaleDuration, _animationSettings.TimerClockHandSpeed, _animationSettings.TimerEase, _animationSettings.ClockEase);
           
         //   _normalTextStyle = _questionTxt.font;

            _answerIcon = new[] { 'a', 'b', 'c', 'd' };
            _coinsDisplay.SetCoins(playersAnswereds.Find(x => x.Name == _currentPlayer).Coins);
            _playerUIPanel.SetPlayers(_playersList,_animationSettings.PlayerUIDuration, _animationSettings.PlayerUIResizeDuration, _animationSettings.PlayerUICoinsShakeDuration, _animationSettings.PlayerUICoinsChangeDuration,_animationSettings.PlayerUICoinsRotateAngle,  _animationSettings.PlayerUIJumpEase,
                _currentPlayer);

            // _waitForStart = waitForStart;
            // _bombCount = boosters.Find(x => x.Type == BoosterType.VocabularyBomb).Count;
            // _freezeCount = boosters.Find(x => x.Type == BoosterType.Freeze).Count;
            _boostersPanels.Init();
            _boostersPanels.HideWrongAnswer += OnHideWrongAnswer;
            _boostersPanels.VaBank += OnVaBanque;
            _boostersPanels.FreezeEnemy += OnFreezeEnemy;
            _boostersPanels.BombEnemy += OnBombEnemy;
            _boostersPanels.DisplayEnemiesAnswers += OnDisplayEnemiesAnswers;
            _boostersPanels.RotateEnemiesAnswers += OnRotateEnemiesAnswers;
            _boostersPanels.ComicEnemiesAnswers += OnComicEnemiesAnswers;
            _boostersPanels.MirrorEnemiesAttack += OnMirrorEnemiesAttack;
        }

        public void InitializeQuiz(QuestionsList questSettingz, int questionAmount)
        {
            _quest = questSettingz.Questions;
            _questionsAmount = questionAmount;
            SetQuestions();
        }

        public async void DisplayRightAnswer(List<PlayersAnswered> playersAnswereds)
        {
           Finish(playersAnswereds);
            await IsContinue();
        }

        public void Finish(List<PlayersAnswered> playersAnswereds)
        {
            _playersList.Clear();
            _playersList = playersAnswereds;
            _playerUIPanel.CountCoins(playersAnswereds);
            DisplayAnswer();
            InscribeHistory();
            _coinsDisplay.ShowCoinsChange(_playersList.Find(x => x.Name == _currentPlayer).Coins);
        }

        public void Attacked(BoosterType boosterType)
        {
            switch (boosterType)
            {
                case BoosterType.Freeze:
                    _attackedFreeze = true;
                    break;
                case BoosterType.VocabularyBomb:
                    _attackedBomb = true;
                    break;
                case BoosterType.Rotate:
                    _attackedRotate = true;
                    break;
                case BoosterType.Comic:
                    _attackedComic = true;
                    break;
                default:
                    Debug.Log("No such attack boost");
                    break;
            }
        }

        public void ShowPlayersAttacks(string enemyName, BoosterType type)
        {
            Debug.Log(type+" TYPE TAKE ATTACK");
            switch (type)
            {
                case BoosterType.Freeze:
                    _playerUIPanel.ShowFreezeOnEnemy(enemyName);
                    break;
                case BoosterType.VocabularyBomb:
                   
                    _playerUIPanel.ShowBombOnEnemy(enemyName);
                    break;
                case BoosterType.Rotate:
                   // _attackedRotate = true;
                    break;
                case BoosterType.Comic:
                  //  _attackedComic = true;
                    break;
             //   case BoosterType.Mirror:
            //     _playerUIPanel.ShowReflect(enemyName);
                //    break;
                default:
                    Debug.Log("No such attack boost");
                    break;
            }
        }

        public void ShowEnemiesAnswers(List<PlayersAnswered> playersAnsweredsList)
        {
            _playerUIPanel.ShowEnemiesAnswers(playersAnsweredsList);
        }

        public void EndingFreeze()
        {
            _playerUIPanel.EndingFreeze();
        }

        public void ReflectingPlayer(string playerName)
        {
            _playerUIPanel.ShowReflect(playerName);
        }

        private void InscribeHistory()
        {
            _answerHistoryPanel.MakeInscribe(_isRight);
        }

        private async Task IsContinue()
        {
            _index++;
            await Task.Delay(1000);
            if (_index+1 >= _quest.Count)
            {
                CompleteQuiz?.Invoke();
            }
            else
            {
                SetQuestions();
            }
        }

        private void SetQuestions()
        {
            _isAnswered = false;
            _restTime = 0;
            _isRight = false;
            _boosterType = BoosterType.None;
            _onHidingAnswer = false;
            _timeIsOut = false;
            _isLate = false;
            // await MakePause();
            _timerView.SetTimer();
            _timerView.Completed += TimerCompleted;
            _numQuestion = _index + 1;
            if (_numQuestion == 7)
            {
              //  _boostersPanels.ShowAllInBooster();
            }
            _questionNumber.text = _numQuestion + "/" + _questionsAmount;
            if (_coinsDisplay == null)
                return;
            _coinsDisplay.Hide();
            if (_changeTextStyle)
            {
                _question.Restart(_normalTextStyle);
                foreach (AnswerView answer in _answers)
                {
                    answer.ChangeFont(_normalTextStyle);
                }
                _changeTextStyle = false;
            }

            _question.SetText(_quest[_index].Quest, _animationSettings.QuestionMoveDuration, _animationSettings.QuestionResizeDuration, _animationSettings.QuestionEase);

            for (int i = 0; i < _answers.Length; i++)
            {
                _answers[i].Initialize(_quest[_index].Answers[i], _animationSettings.AnswerScaleDuration, _animationSettings.AnswerClickedDuration,_animationSettings.AnswerClickedScale, _animationSettings.AnswerEase,_answerIcon[i], i);
                _answers[i].MakeAnswer += PlayerAnswer;
            }

            TakeHitAttacks();
        }

        private void MakePause()
        {
          //  _questionTxt.gameObject.SetActive(false);
            _coinsDisplay.Hide();
            foreach (AnswerView answer in _answers)
            {
                answer.MakePause(false);
            }

            // await Task.Delay((int)_waitForStart * 1000);
          //  _questionTxt.gameObject.SetActive(true);
            foreach (AnswerView answer in _answers)
            {
                answer.MakePause(true);
            }
        }

        private void TakeHitAttacks()
        {
            if (_mirror)
            {
                _attackedFreeze = false;
                _attackedBomb = false;
                _attackedRotate = false;
                _attackedComic = false;
                _mirror = false;
            }
            if (_attackedFreeze)
            {
                MakeFreeze?.Invoke(_freezeTimer);
               // _playerUIPanel.TakeFreeze();
               // HideAnswers(_freezeTimer);
                _attackedFreeze = false;
            }

            if (_attackedBomb)
            {
                MakeVocabularyBomb();
                _attackedBomb = false;
            }

            if (_attackedRotate)
            {
                MakeRotateQuestion();
                _attackedRotate = false;
            }

            if (_attackedComic)
            {
                MakeComicQuestionAndAnswers();
                _attackedComic = false;
            }
            _playerUIPanel.TakeAttacking(_freezeTimer);
        }

        private void TimerCompleted()
        {
            _timerView.Completed -= TimerCompleted;
            _timeIsOut = true;
            if (!_isAnswered)
            {
                _isLate = true;
                var randAnswer = Random.Range(0, _answers.Length);
                _answers[randAnswer].MakeRandomAnswer();
            }
            else
            {
                foreach (AnswerView answer in _answers)
                {
                    answer.MakeAnswer -= PlayerAnswer;
                    answer.Clear();
                }

                PlayerMakeAnswer?.Invoke(_isRight, _quest[_index].Coins + _restTime);
            }
        }


        private void PlayerAnswer(bool isRighnt, AnswerView answerView, char icon)
        {
            _isAnswered = true;
            _restTime = _timerView.RestTime;
            _isRight = isRighnt;
            foreach (AnswerView answer in _answers)
            {
                if (answer != answerView)
                    answer.NotClicked();
            }

            if (_isLate)
            {
                foreach (AnswerView answer in _answers)
                {
                    answer.MakeAnswer -= PlayerAnswer;
                    answer.Clear();
                }

                PlayerMakeAnswer?.Invoke(isRighnt, _quest[_index].Coins + _restTime);
            }
            else
            {
                PlayerChoose?.Invoke(icon);
            }
        }

        private void DisplayAnswer()
        {
            _boosterType = BoosterType.Loupe;
            for (int i = 0; i < _quest[_index].Answers.Count; i++)
            {
                if (_quest[_index].Answers[i].RightVariant)
                    _answers[i].DisplayRight();
                else
                    _answers[i].DisplayYourError();
            }

            _boostersPanels.InactiveButtonSprite(_boosterType);
        }

        private void OnHideWrongAnswer()
        {
            //  _boostersPanels.HideWrongAnswer -= OnHideWrongAnswer;
            if (_boosterType == BoosterType.ShowHalf || _onHidingAnswer)
                return;
            _boosterType = BoosterType.ShowHalf;
            _onHidingAnswer = true;
            int hideAnswer = 0;
            while (hideAnswer != 2)
            {
                int rnsAnswer = Random.Range(0, _quest[_index].Answers.Count);
                if (!_quest[_index].Answers[rnsAnswer].RightVariant && _answers[rnsAnswer].gameObject.activeInHierarchy)
                {
                    _answers[rnsAnswer].Hide();
                    hideAnswer++;
                }
            }

            _boostersPanels.InactiveButtonSprite(_boosterType);
        }

        private void OnVaBanque()
        {
            PlayerBanque?.Invoke();
        }

        private void OnFreezeEnemy()
        {
            _boosterType = BoosterType.Freeze;
            _playerUIPanel.PlayerChooseEnemyToAttack += MakeAttackEnemy;
            _playerUIPanel.DisplayEnemiesEnabled();
        }

        private void OnBombEnemy()
        {
            _boosterType = BoosterType.VocabularyBomb;
            _usedAttackBooster = false;
            _playerUIPanel.PlayerChooseEnemyToAttack += MakeAttackEnemy;
            _playerUIPanel.DisplayEnemiesEnabled();
        }

        private void OnRotateEnemiesAnswers()
        {
            _boosterType = BoosterType.Rotate;
            _usedAttackBooster = false;
            _playerUIPanel.PlayerChooseEnemyToAttack += MakeAttackEnemy;
            _playerUIPanel.DisplayEnemiesEnabled();
        }

        private void OnComicEnemiesAnswers()
        {
            _boosterType = BoosterType.Comic;
            _usedAttackBooster = false;
            _playerUIPanel.PlayerChooseEnemyToAttack += MakeAttackEnemy;
            _playerUIPanel.DisplayEnemiesEnabled();
        }

        private void OnMirrorEnemiesAttack()
        {
            _mirror = true;
            _boosterType = BoosterType.Mirror;
            _boostersPanels.InactiveButtonSprite(_boosterType);
            ReflectAttack?.Invoke();
        }

        private void MakeAttackEnemy(string enemyName)
        {
            _usedAttackBooster = true;
            _playerUIPanel.PlayerChooseEnemyToAttack -= MakeAttackEnemy;
            _boostersPanels.InactiveButtonSprite(_boosterType);
            PlayerChooseEnemyToAttack?.Invoke(enemyName, _boosterType);
        }


        private void OnDisplayEnemiesAnswers()
        {
            //   _boostersPanels.DisplayEnemiesAnswers -= OnDisplayEnemiesAnswers;
            _boosterType = BoosterType.Loupe;
            DisplayAllEnemiesAnswers?.Invoke();
        }

        private void MakeVocabularyBomb()
        {
            _question.MakeBomb();
        }

        private void MakeRotateQuestion()
        {
            _question.RotateText(_animationSettings.QuestionMoveDuration);
            _changeTextStyle = true;
        }

        private void MakeComicQuestionAndAnswers()
        {
            _question.ChangeFont(_comicSansFont);
            foreach (AnswerView answer in _answers)
            {
                answer.ChangeFont(_comicSansFont);
            }
            _changeTextStyle = true;
        }

        private void HideAnswers(float freezeTimer)
        {
            foreach (AnswerView answer in _answers)
            {
                 StopCoroutine(answer.Hide(freezeTimer));
                StartCoroutine(answer.Hide(freezeTimer));
            }
        }
    }
}