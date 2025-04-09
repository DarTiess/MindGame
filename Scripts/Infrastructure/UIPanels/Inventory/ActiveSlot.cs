using System;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.UIPanels.Inventory.Dragged;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.Inventory
{
    public class ActiveSlot : SlotBase, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private TMP_Text _boosterName;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _count;


        private bool _isUsed;
        private SlotDraggedView _slotDragged;
        private int _countInt;
        public bool _isDragged;
        public bool Active => _usedSlot;


        public void Initialize(Booster booster, SlotDraggedView slotDragged)
        {
            _booster = booster;
            _boosterName.text = booster.BoosterName;
            _description.text = booster.Description;
            _icon.sprite = booster.Icon;
            _isUsed = booster.IsUsed;
            _count.text = booster.Count.ToString();
            _countInt = booster.Count;
            _count.transform.parent.gameObject.SetActive(true);
            _usedSlot = true;
            _slotDragged = slotDragged;
            ChangeBack(_backDefault);
            // _count = booster.Count;
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


        public void ShowActiveSlot(bool use)
        {
            // _booster.IsUsed = use;
              _usedSlot = use;
            _boosterName.transform.parent.gameObject.SetActive(use);
            _backEmpty.SetActive(!use);
            _backSelected.SetActive(use);
        }

        public void CheckUsefull()
        {
            if (_booster.IsUsed)
            {
                
                ChangeBack(_backFadeSelected);
            }
            else
            {
                ChangeBack(_backDefault);
            }
        }

        public void UpdateInfo(int objBoosterCount)
        {
            _countInt += objBoosterCount;
            _count.text = _countInt.ToString();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_usedSlot)
            {
                _count.transform.parent.gameObject.SetActive(false);
                ChangeBack(_backEmpty);
                OnClicked(this);
                _isDragged = true;
                _slotDragged.transform.position = eventData.position;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isDragged)
                _slotDragged.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isDragged)
            {
                _slotDragged.EndDrag(eventData);
                _isDragged = false;
            }
        }
    }
}