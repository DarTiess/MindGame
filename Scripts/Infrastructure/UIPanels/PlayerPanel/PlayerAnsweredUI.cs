using System;
using DG.Tweening;
using Network.NetworkObjects;
using Spine;
using Spine.Unity;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.PlayerPanel
{
    public class PlayerAnsweredUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _playerName;
        [SerializeField] private TMP_Text _playerCoinsTxt;
       // [SerializeField] private Image _playerAvatar;
        [SerializeField] private Image _frameAvatar;
        [SerializeField] private Sprite _mainPlayerIcon;
        [SerializeField] private Sprite _enemyIcon;
        [SerializeField] private Button _clickedButton;
        [SerializeField] private Sprite _lightOnEnemySprite;
        [SerializeField] private TMP_Text _answerIconTxt;
        [SerializeField] private Image _answerIcon;
        [SerializeField] private SkeletonView _skeletonView;

        private float _coinsAmount;
        private float _moveDuration;
        private float _coinsRotateAngle;
        private string _mainPlayer;
        private Ease _ease;
        private Sprite _normalEnemySprite;
        private RectTransform _rect;
        private float _resizeDuration;
        private float _coinsShakeDuration;
        private float _textDuration;
        private bool _isBombed;
        private bool _isFreezed;
        private bool _isReflect;
        public string PlayerName => _playerName.text;

        public event Action<string> OnMakeChooseEnemy;

        public void SetPlayer(PlayersAnswered playersAnswered, float moveDuration,
            float resizeDuration, float coinsShakeDuration, float coinsChangeDuration,
            float rotateAngle, Ease jumpEase, string currentPlayer, int timer)
        {
            if (gameObject == null || playersAnswered == null)
                return;

            gameObject.SetActive(true);
            _clickedButton = GetComponent<Button>();
            _rect = _frameAvatar.GetComponent<RectTransform>();
            _clickedButton.enabled = false;
            _moveDuration = moveDuration;
            _resizeDuration = resizeDuration;
            _coinsShakeDuration = coinsShakeDuration;
            _textDuration = coinsChangeDuration;
            _coinsRotateAngle = rotateAngle;
            _ease = jumpEase;
            _playerName.text = playersAnswered.Name;
            _skeletonView.SetSkeletonSkin(playersAnswered.GetAvatarSprite());
            _skeletonView.StartAnimation(timer);

            _mainPlayer = currentPlayer;
            _coinsAmount = playersAnswered.Coins;
            _answerIcon.gameObject.SetActive(false);
            _playerCoinsTxt.text = _coinsAmount.ToString();
            if (_mainPlayer == PlayerName)
            {
                PlayerIcon(_mainPlayerIcon);
            }
        }

    
        public void LightOn()
        {
            _clickedButton.enabled = true;
            _clickedButton.onClick.AddListener(ChooseEnemy);
            _frameAvatar.sprite = _lightOnEnemySprite;
        }

        public void LightOff()
        {
            _clickedButton.enabled = false;
            _clickedButton.onClick.RemoveListener(ChooseEnemy);
            _frameAvatar.sprite = _enemyIcon;
        }

        public void MoveTo(Vector3 playerAnsweredUiPlace, PlayersAnswered playersAnswered)
        {
            _rect.DOSizeDelta(new Vector2(152f, 1f), 0.1f)
                .OnStart(() =>
                {
                    _playerName.gameObject.SetActive(false);
                    _playerCoinsTxt.gameObject.SetActive(false);
                })
                .OnComplete(() =>
                {
                    transform.DOMove(playerAnsweredUiPlace, _moveDuration)
                        .OnStart(() =>
                        {
                            if(playerAnsweredUiPlace.x>transform.position.x)
                                _skeletonView.Jump(-1);
                            else if(playerAnsweredUiPlace.x<transform.position.x)
                                _skeletonView.Jump(1);
                        })
                        .SetEase(_ease)
                        .OnComplete(() =>
                        {
                            _skeletonView.IdleAnimation();
                            _rect.DOSizeDelta(new Vector2(152f, 27.4f), _resizeDuration)
                                .OnComplete(() =>
                                {
                                    _playerName.gameObject.SetActive(true);
                                    _playerCoinsTxt.gameObject.SetActive(true);
                                });
                            CountCoins(playersAnswered);
                        });
                });
          
        }

        public void ShowAnswer(char answerIcon)
        {
            _answerIcon.gameObject.SetActive(true);
            _answerIconTxt.text = answerIcon.ToString();
        }

        private void CountCoins(PlayersAnswered playersAnswered)
        {
            if (_coinsAmount < playersAnswered.Coins)
            {
                _playerCoinsTxt.transform.DOShakeRotation(_coinsShakeDuration, _coinsRotateAngle);
            }

            _playerName.text = playersAnswered.Name;
            if (_mainPlayer == PlayerName)
            {
                PlayerIcon(_mainPlayerIcon);
            }
            else
            {
                PlayerIcon(_enemyIcon);
            }

            _coinsAmount = playersAnswered.Coins;
          //  _playerCoinsTxt.DOText(playersAnswered.Coins.ToString(), _textDuration);
            _playerCoinsTxt.text=playersAnswered.Coins.ToString();
            _answerIcon.gameObject.SetActive(false);
        }

        private void PlayerIcon(Sprite icon)
        {
            _frameAvatar.sprite = icon;
        }

        private void ChooseEnemy()
        {
           // _skeletonView.IdleBombAnimation();
            OnMakeChooseEnemy?.Invoke(PlayerName);
        }

        public void TakeFreeze()
        {
            _skeletonView.FreezeAnimation(7f);
        }

        public void TakeAttack(float freezeTimer)
        {
            if(_isReflect)
            {
                _isReflect = false;
                _isBombed = false;
                _isFreezed = false;
                _skeletonView.Reflecting();
                return;
            }
            if (_isBombed)
            {
                _skeletonView.BombAnimation();
                _isBombed = false;
            }

            if (_isFreezed)
            {
                _skeletonView.FreezeAnimation(freezeTimer);
                _isFreezed = false;
            }
        }

        public void ShowBombOnEnemy()
        {
            if(_isReflect)
                return;
            
            _skeletonView.IdleBombAnimation();
            _isBombed = true;
        }

        public void ShowFreezeOnEnemy()
        {
            if(_isReflect)
                return;
            
            _skeletonView.IdleFreezeAnimation();
            _isFreezed = true;
        }

        public void ShowReflect()
        {
           // idle reflect
           _isReflect = true;
        }
    }
}