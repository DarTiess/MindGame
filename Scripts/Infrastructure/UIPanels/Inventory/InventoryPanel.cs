using Infrastructure.EventsBus;
using Infrastructure.EventsBus.Signals;
using Infrastructure.Installers.Settings.Build;
using Infrastructure.Installers.Settings.Players;
using Infrastructure.UIPanels.Inventory.Dragged;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

namespace Infrastructure.UIPanels.Inventory
{
    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private Button _closeButton;
        [FormerlySerializedAs("_inventorySlotPrefab")] [SerializeField] private InactiveSlot inactiveSlotPrefab;
        [SerializeField] private SlotDraggedView _slotDragViewPrefab;
        [SerializeField] private ActiveSlotPanel _activeSlotPanel;
        [SerializeField] private InactiveSlotPanel _inactiveSlotPanel;

        private SlotDraggedView _slotDragView;
        private IEventBus _eventBus;
        private IInventoryService _inventoryService;

        [Inject]
        public void Construct(IInventoryService inventoryService, IEventBus eventBus)
        {
            _inventoryService = inventoryService;
            _eventBus = eventBus;
        }

        private void Start()
        {
            _eventBus.Subscribe<BuyBooster>(UpdateBoostersInactive);

            _closeButton.onClick.AddListener(Hide);
            _slotDragView = Instantiate(_slotDragViewPrefab, transform);
            _slotDragView.Hide();

            _activeSlotPanel.Initialize(_inventoryService, _slotDragView);
            _activeSlotPanel.ClickedActiveSlot += ClickedActiveSlot;
            _activeSlotPanel.RemoveActiveSlot += UpdateInventoryPanel;

            _inactiveSlotPanel.Initialize(_inventoryService, inactiveSlotPrefab, _slotDragView);
            _inactiveSlotPanel.ClickedInactiveSlot += ClickedInactiveSlot;

            Hide();
        }

        private void ClickedActiveSlot(SlotBase slot)
        {
            _slotDragView.InitActiveSlot(slot, _activeSlotPanel, _inactiveSlotPanel.GetComponent<RectTransform>());
        }

        private void UpdateBoostersInactive(BuyBooster obj)
        {
            _inactiveSlotPanel.UpdateBooster(obj.Booster);
            _activeSlotPanel.UpdateBooster(obj.Booster);
        }

        private void UpdateInventoryPanel()
        {
            _inactiveSlotPanel.UpdateInactivePanel();
        }

        private void ClickedInactiveSlot(SlotBase slot)
        {
            _slotDragView.InitInactiveSlot(slot, _activeSlotPanel, _inactiveSlotPanel.GetComponent<RectTransform>());
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