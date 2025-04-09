using System;
using Infrastructure.Installers.Settings.Players;
using UnityEngine;

namespace Infrastructure.UIPanels.Inventory
{
    public class SlotBase : MonoBehaviour
    {
        [SerializeField] protected GameObject _backSelected;
        [SerializeField] protected GameObject _backLocked;
        [SerializeField] protected GameObject _lockeIcon;
        [SerializeField] protected GameObject _backDefault;
        [SerializeField] protected GameObject _backEmpty;
        [SerializeField] protected GameObject _backFadeSelected;
        protected GameObject _currentBack;
        protected Booster _booster;
        protected bool _usedSlot;

        public Booster BoosterInfo => _booster;

        public event Action<SlotBase> ReturnSlot;
        public event Action<SlotBase> Clicked;


        public void Return()
        {
            ChangeBack(_backDefault);
           // _usedSlot = false;
            ReturnSlot?.Invoke(this);
        }
        protected void ChangeBack(GameObject back)
        {
            if (_currentBack != null)
            {
                _currentBack.SetActive(false);
            }

            _currentBack = back;
            _currentBack.SetActive(true);
        }

        protected  void OnClicked(SlotBase obj)
        {
            Clicked?.Invoke(obj);
        }
    }
}