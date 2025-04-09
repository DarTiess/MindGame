using System.Collections.Generic;
using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Bots;
using Infrastructure.Installers.Settings.MainPlayer;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.UIPanels.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Infrastructure.UIPanels
{
    public class ShopPanel: MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Transform _container;
        [SerializeField] private ShopSlot _slotPrefab;
        private List<ShopSlot> _slots;
        private ShopsService _shopsService;
        private IMainPlayerLoader _mainPlayerLoader;
        private IEventBus _eventBus;

        [Inject]
        public void Construct(ShopsService shopsService, IEventBus eventBus, IMainPlayerLoader mainPlayerLoader)
        {
            _shopsService = shopsService;
            _eventBus = eventBus;
            _mainPlayerLoader = mainPlayerLoader;
        }

        private void Start()
        {
             _slots = new List<ShopSlot>();
           foreach (Booster booster in _shopsService.Boosters)
           {
               if (booster.BoosterName != "Soft")
               {
                   ShopSlot boost = Instantiate(_slotPrefab, _container);
                   boost.Initialize(booster);
                   boost.Clicked += OnClickedSlot;
                   if (!booster.IsOpened ||_mainPlayerLoader.PlayerCoins < booster.Prize)
                   {
                       boost.SetInactiveSlot();
                   }
                   _slots.Add(boost);
               }
           }

           _closeButton.onClick.AddListener(Hide);
           Hide();
        }

        private void OnClickedSlot(ShopSlot slot,Booster booster)
        {
            slot.Clicked -= OnClickedSlot;
            _eventBus.Invoke(new BuyBooster(booster));
            foreach (ShopSlot shopSlot in _slots)
            {
                if (!booster.IsOpened || _mainPlayerLoader.PlayerCoins < shopSlot.Prize)
                {
                    shopSlot.SetInactiveSlot();
                }
            }
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}