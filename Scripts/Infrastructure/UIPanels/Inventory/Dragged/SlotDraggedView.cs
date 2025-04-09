using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Infrastructure.UIPanels.Inventory.Dragged
{
    public class SlotDraggedView : MonoBehaviour
    {
        [Header("Components")] 
        [SerializeField] private TMP_Text _boosterName;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Image _icon;

        private bool _isSelected;
        private ActiveSlotPanel _activeSlot;
        private RectTransform _activeSlotPosition;
        private SlotBase _currentSlot;
        private RectTransform _inactiveSlotPosition;
        private bool _isActive;

        public void InitInactiveSlot(SlotBase slot, ActiveSlotPanel activeSlot, RectTransform inactiveSlot)
        {
            Debug.Log("Init Drag");
            _currentSlot = slot;
            transform.position = _currentSlot.transform.position;
            _activeSlot = activeSlot;
            _activeSlotPosition = activeSlot.GetComponent<RectTransform>();
            _inactiveSlotPosition = inactiveSlot;
            _boosterName.text = _currentSlot.BoosterInfo.BoosterName;
            _description.text = _currentSlot.BoosterInfo.Description;
            _icon.sprite =  _currentSlot.BoosterInfo.Icon;
            gameObject.SetActive(true);
        }

        public void InitActiveSlot(SlotBase slot, ActiveSlotPanel activeSlot, RectTransform inactiveSlot)
        {
            Debug.Log("Init Drag");
            _currentSlot = slot;
            transform.position = _currentSlot.transform.position;
            _activeSlot = activeSlot;
            _activeSlotPosition = activeSlot.GetComponent<RectTransform>();
            _inactiveSlotPosition = inactiveSlot;
            _boosterName.text = _currentSlot.BoosterInfo.BoosterName;
            _description.text = _currentSlot.BoosterInfo.Description;
            _icon.sprite =  _currentSlot.BoosterInfo.Icon;
            _isActive = true;
            gameObject.SetActive(true);
        }


        public void Hide()
        {
            Debug.Log("Hide slot dragged");
           
            gameObject.SetActive(false);
        }


        public void EndDrag(PointerEventData eventData)
        {
            Debug.Log("CLOCIKKJF");
            if(_currentSlot==null)
                return;
            if (!_isActive)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_activeSlotPosition, eventData.position, eventData.pressEventCamera))
                {
                    if (_activeSlot.IsFulled)
                    {
                        _currentSlot.Return();
                        _currentSlot = null;
                    }
                    else
                    {
                        _activeSlot.AddToCollection(_currentSlot.BoosterInfo);
                        _currentSlot = null;
                    }
                }
                else
                {
                    _currentSlot.Return();
                    _currentSlot = null;
                } 
            }
            else
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_inactiveSlotPosition, eventData.position, eventData.pressEventCamera))
                {
                    _activeSlot.RemoveInCollection(_currentSlot.BoosterInfo);
                    //make in inactive panel show card
                   _currentSlot = null;
                }
                else
                {
                    _currentSlot.Return();
                    _currentSlot = null;
                }  
                _isActive = false;
            }
           
            Hide();
        }
    }
}

