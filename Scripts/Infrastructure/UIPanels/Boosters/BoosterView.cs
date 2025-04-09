using System;
using Infrastructure.Installers.Settings.Players;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.Boosters
{
    public class BoosterView: MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _icon;
        private BoosterType _type;
        private Sprite _activeButtonSprite;
        private bool _clicked;
        private int _count;
        private Booster _booster;
        private bool _allIn;
        public BoosterType Type => _type;
        public event Action<BoosterType, BoosterView> Clicked;

        public void Initialize(Booster booster, Sprite activeButtonSprite)
        {
            _button.onClick.AddListener(ClickedBooster);
            _type = booster.Type;
            _icon.sprite = booster.Icon;
            _activeButtonSprite = activeButtonSprite;
            _count = 1;
            _booster = booster;
            gameObject.SetActive(true);
        }

        private void ClickedBooster()
        {
          //  if (_type == BoosterType.VocabularyBomb || _type == BoosterType.Freeze)
                _button.image.sprite = _activeButtonSprite;
          //  else
               // MarkUsedBooster();

            _clicked = true;
            Clicked?.Invoke(_type, this);
        }

        public void ActivateButton()
        {
            _button.interactable = true;
        }

        public void InactiveButton(Sprite inactiveButtonSprite, BoosterType boosterType)
        {
            if (_clicked && _type==boosterType)
            {
                _button.image.sprite = inactiveButtonSprite;
                MarkUsedBooster();
            }
        }

        private void MarkUsedBooster()
        {
            if (_count <= 0)
                return;
            
            _count--;
            //_button.interactable = false;
            if (_count <= 0)
            {
                _button.interactable = false;
            }

            else
            {
                _clicked = false;
            }

            if(!_allIn)
                _booster.RemoveBooster();
        }

        public void InitializeAllInBooster(Sprite activeButtonSprite)
        {
            _button.onClick.AddListener(ClickedBooster);
            _type =BoosterType.AllIn;
            //_icon.sprite = booster.Icon;
            _activeButtonSprite = activeButtonSprite;
            _count = 1;
            _allIn = true;
            // _booster = booster;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}