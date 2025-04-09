using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using Infrastructure.Installers.Settings.Quiz;
using Network.NetworkObjects;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.Answers
{
    public class AnswerView : MonoBehaviour
    {
        [FormerlySerializedAs("answerText")] [SerializeField] private TextMeshProUGUI _answerText;
        [SerializeField] private Color rightColor;
        [SerializeField] private Color startColor;
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private Sprite errorSprite;
        [SerializeField] private Sprite startSprite;
        [SerializeField] private GameObject _freezeCard;
        [SerializeField] private GameObject _errorIcon;
        [SerializeField] private GameObject _rightIcon;
         private float _selectedScale;

        private Button _button;
        private Image _backGround;
        private Answer _answer;
        private bool _isPressed;
        private char _icon;
        private bool _started;
        private Vector3 _startingScale;
        private float _clickedDuration;

        public event Action<bool, AnswerView, char> MakeAnswer;

        private void OnEnable()
        {
            _backGround = GetComponent<Image>();
            _backGround.color = startColor;
            _backGround.sprite = startSprite;
           
            _freezeCard.SetActive(false);
        }

        public async void Initialize(Answer answer, float scaleDuration, float clickedDuration,float clickedScale, Ease scaleEase,
            char icon, int i)
        {
            gameObject.SetActive(true);
            _answer = answer;
            _clickedDuration = clickedDuration;
            _selectedScale = clickedScale;
            _icon = icon;
            _answerText.text = answer.Description;
            _backGround = GetComponent<Image>();
            _backGround.color = startColor;
            _backGround.sprite = startSprite;
            _errorIcon.SetActive(false);
            _rightIcon.SetActive(false);
            _button = GetComponent<Button>();
            _button.onClick.AddListener(PlayerMakeAnswer);
            SwitchEnable(true);

            if (!_started)
            { 
                _startingScale = transform.localScale;
                transform.localScale=Vector3.zero;
                await Task.Delay(i * 150);
                gameObject.transform.DOScale(_startingScale, scaleDuration).SetEase(scaleEase);
                _started = true;
            }
            else
            {
                transform.localScale=Vector3.zero;
                gameObject.transform.DOScale(_startingScale, scaleDuration).SetEase(scaleEase);
                //.SetLoops(2, LoopType.Yoyo);

            }
          //  foreach (var players in playersAnswereds)
         //   {
              //  SetPoints(playerPoint, players);
          //  }
        }

        public void DisplayRight()
        {
            _backGround.color = rightColor;
            _isPressed = false;
            _answerText.color=Color.white;

            _rightIcon.SetActive(true);
            transform.DOScale(new Vector3(_startingScale.x + _selectedScale, _startingScale.y + _selectedScale), _clickedDuration);
        }

        public void DisplayYourError()
        {
            if (_isPressed)
            {
                _backGround.sprite = errorSprite;
                _errorIcon.SetActive(true);
                _isPressed = false;
                _answerText.color=Color.white;
                transform.localScale=_startingScale;

            }
        }

        public void MakeRandomAnswer()
        {
            PlayerMakeAnswer();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void NotClicked()
        { 
            _isPressed = false;
            _backGround.color = startColor;
            _backGround.sprite = startSprite;
            _answerText.color=Color.black;

            transform.localScale= _startingScale;

        }

        public IEnumerator Hide(float freezeTimer)
        {
          //  answerText.enabled = false;
          _freezeCard.SetActive(true);
            yield return new WaitForSeconds(freezeTimer);
            _freezeCard.SetActive(false);
          //  answerText.enabled = true;
        }

        public void Clear()
        {
            _button.onClick.RemoveListener(PlayerMakeAnswer);
        }

        public void MakePause(bool enable)
        {
            _answerText.enabled = enable;
            _backGround.color = startColor;
            _backGround.sprite = startSprite;
        }

        public void ChangeFont(TMP_FontAsset font)
        {
            _answerText.font = font;
        }

        private void SwitchEnable(bool isActive)
        {
            _button.interactable = isActive;
            _answerText.color=Color.black;
            if(_started)
                transform.localScale= _startingScale;

        }

        private void SetPoints(TextMeshProUGUI point, PlayersAnswered player)
        {
            point.color = Color.cyan;
            point.text = player.Name.Substring(0, 1);
            HideObject(point);
        }

        private void HideObject(TextMeshProUGUI textMeshProUGUI)
        {
            if (textMeshProUGUI != null)
                textMeshProUGUI.gameObject.SetActive(false);
        }

        private void PlayerMakeAnswer()
        {
            _isPressed = true;
            if (_backGround == null)
                return;
            _backGround.sprite = selectedSprite;
            _answerText.color=Color.white;

            transform.DOScale(new Vector3(_startingScale.x + _selectedScale, _startingScale.y + _selectedScale), _clickedDuration);
           // transform.localScale= new Vector3(_startingScale.x+_selectedScale, _startingScale.y+_selectedScale);
            MakeAnswer?.Invoke(_answer.RightVariant, this, _icon);
        }
    }
}