using System;
using System.Collections.Generic;
using Infrastructure.Installers.Settings.Build;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.UIPanels.Inventory.Dragged;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Infrastructure.UIPanels.Inventory
{
    public class ActiveSlotPanel : MonoBehaviour
    {
        [SerializeField] private List<ActiveSlot> _activeSlots;
        private int _indexSlots;
        private SlotDraggedView _slotDraggedView;
        private IInventoryService _inventoryService;
        public event Action RemoveActiveSlot;
        public event Action<SlotBase> ClickedActiveSlot;
        public bool IsFulled => _indexSlots >= _activeSlots.Count;

        public void Initialize(IInventoryService inventoryService, SlotDraggedView slotDraggedView)
        {
            _inventoryService = inventoryService;
            _slotDraggedView = slotDraggedView;
            ShowActiveSlots();
        }

        private void ShowActiveSlots()
        {
            _indexSlots = 0;
            for (int i = 0; i < _inventoryService.Boosters.Count; i++)
            {
                if (_indexSlots >= _activeSlots.Count)
                    return;
                if (_inventoryService.Boosters[i].IsUsed)
                {
                    _activeSlots[_indexSlots].Initialize(_inventoryService.Boosters[i], _slotDraggedView);
                    _activeSlots[_indexSlots].Clicked += OnClickedSlot;
                    _activeSlots[_indexSlots].ShowActiveSlot(true);

                    _indexSlots++;
                }
            }
        }

        public void AddToCollection(Booster booster)
        {
            booster.IsUsed = true;
            ShowActiveSlots();
        }

        private void OnClickedSlot(SlotBase slot)
        {
            slot.Clicked -= OnClickedSlot;
            slot.ReturnSlot += OnReturnSlot;
            ClickedActiveSlot?.Invoke(slot);
        }

        private void OnReturnSlot(SlotBase slot)
        {
            slot.ReturnSlot -= OnReturnSlot;
            slot.Clicked += OnClickedSlot;
        }

        public void RemoveInCollection(Booster booster)
        {
            booster.IsUsed = false;

            _activeSlots.ForEach(x=>x.ShowActiveSlot(false));
           ShowActiveSlots();
            RemoveActiveSlot?.Invoke();
        }

        public void UpdateBooster(Booster objBooster)
        {
            if(_indexSlots<=0)
                return;
            if (_activeSlots.Exists(x => x.BoosterInfo.BoosterName == objBooster.BoosterName && x.Active))
            {
                Debug.Log("HAS BOOSTER ACTIV");
                _activeSlots.Find(x => x.BoosterInfo.BoosterName == objBooster.BoosterName).UpdateInfo(objBooster.Count); 
            }
        }
    }
}