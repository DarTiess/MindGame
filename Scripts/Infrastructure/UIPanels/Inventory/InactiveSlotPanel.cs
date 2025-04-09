using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.Build;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.UIPanels.Inventory.Dragged;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Infrastructure.UIPanels.Inventory
{
    public class InactiveSlotPanel : MonoBehaviour
    {
        [SerializeField] private Transform _container;

        private List<InactiveSlot> _slots;
        private InactiveSlot _inactiveSlotPrefab;
        private SlotDraggedView _slotDraggedView;
        private IInventoryService _inventoryService;
        public event Action<SlotBase> ClickedInactiveSlot;

        public void Initialize(IInventoryService inventoryService, InactiveSlot inactiveSlotPrefab,
            SlotDraggedView slotDraggedView)
        {
            _slots = new List<InactiveSlot>();
            _inventoryService = inventoryService;
            _inactiveSlotPrefab = inactiveSlotPrefab;
            _slotDraggedView = slotDraggedView;
            foreach (Booster booster in _inventoryService.Boosters)
            {
                var boost = Instantiate(inactiveSlotPrefab, _container);
                boost.Initialize(booster, _slotDraggedView);
                boost.CheckUsefull();
                boost.Clicked += OnClickedInventorySlot;
                _slots.Add(boost);
            }
        }

        private void OnClickedInventorySlot(SlotBase slot)
        {
            //_slotDragView.Init(slot, _activeSlotPanel, this);
            ClickedInactiveSlot?.Invoke(slot);
            slot.Clicked -= OnClickedInventorySlot;
            slot.ReturnSlot += OnReturnSlot;
        }

        private void OnReturnSlot(SlotBase slot)
        {
            slot.ReturnSlot -= OnReturnSlot;
            slot.Clicked += OnClickedInventorySlot;
        }

        public void UpdateInactivePanel()
        {
            foreach (InactiveSlot slot in _slots)
            {
                slot.CheckUsefull();
                slot.Clicked += OnClickedInventorySlot;
            }
        }

        public void AddBooster(Booster booster)
        {
            var boost = Instantiate(_inactiveSlotPrefab, _container);
            boost.Initialize(booster, _slotDraggedView);
            boost.CheckUsefull();
            boost.Clicked += OnClickedInventorySlot;
            _slots.Add(boost);
        }

        public void UpdateBooster(Booster objBooster)
        {
            if (_slots.Exists(x => x.BoosterInfo.BoosterName == objBooster.BoosterName))
            {
               _slots.Find(x => x.BoosterInfo.BoosterName == objBooster.BoosterName).UpdateInfo(objBooster.Count); 
            }
            else
            {
                AddBooster(objBooster);
            }
        }
    }
}