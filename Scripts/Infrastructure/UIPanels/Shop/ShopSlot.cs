using System;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.Installers.Settings.Players;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Infrastructure.UIPanels
{
    [RequireComponent(typeof(Button))]
    public class ShopSlot : MonoBehaviour
    {
        [SerializeField] private TMP_Text _boosterName;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _prize;
        [SerializeField] private TMP_Text _count;
        [SerializeField] private GameObject _backSelected;
        [SerializeField] private GameObject _backLocked;
        [SerializeField] private GameObject _lockeIcon;
        [SerializeField] private GameObject _backDefault;
        [SerializeField] private GameObject _backEmpty;
        [SerializeField] private GameObject _backFadeSelected;
        public int Prize => _booster.Prize;
        private Booster _booster;
        private Button _button;
        private GameObject _currentBack;
        private bool _clickable;
        public event Action<ShopSlot,Booster> Clicked;

        public void Initialize(Booster booster)
        {
            _booster = booster;
            _boosterName.text = booster.BoosterName;
            _description.text = booster.Description;
            _icon.sprite = booster.Icon;
            _prize.text = booster.Prize.ToString();
            _count.text = booster.Count.ToString();
            _button = GetComponent<Button>();
            _button.onClick.AddListener(ChooseBooster);
            _lockeIcon.SetActive(false);
            _clickable = true;
            ChangeBack(_backDefault);
        }

        private void ChooseBooster()
        {
            if(!_clickable)
                return;
            ChangeBack(_backFadeSelected);
            _clickable = false;
            Clicked?.Invoke(this,_booster);
        }

        public void SetInactiveSlot()
        {
            ChangeBack(_backLocked);
            _clickable = false;
            _lockeIcon.SetActive(true);
        }

        private void ChangeBack(GameObject back)
        {
            if (_currentBack != null)
            {
                _currentBack.SetActive(false);
            }

            _currentBack = back;
            _currentBack.SetActive(true);
        }
    }
}