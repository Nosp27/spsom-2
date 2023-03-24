using System.Collections.Generic;
using UI.Inventory.ItemViewOrganizers;
using UI.Inventory.ItemViews;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Inventory.GUIMediators
{
    public class SwapSlotGuiMediator : MonoBehaviour
    {
        [SerializeField] private AbstractSlotCanvasOrganizer[] organizers;
        [SerializeField] private Transform itemContainersRoot;

        [SerializeField] private bool allowSelectEmpty;

        public UnityEvent onRunGui { get; private set; }
        public UnityEvent onStopGui { get; private set; }

        private int activeCanvasIdx;
        private ItemView viewFrom;

        private void Awake()
        {
            onRunGui = new UnityEvent();
            onStopGui = new UnityEvent();
        }

        public void Run()
        {
            foreach (AbstractSlotCanvasOrganizer organizer in organizers)
            {
                organizer.gameObject.SetActive(true);
                organizer.Arrange();
                organizer.SetClickListener(ItemViewClicked);
            }
            DumpToUI();
        }

        public void Stop()
        {
            DumpToInventory();
            foreach (AbstractSlotCanvasOrganizer organizer in organizers)
                organizer.gameObject.SetActive(false);
        }

        public void SwitchTab(int delta)
        {
            foreach (var organizer in organizers)
            {
                organizer.Freeze();
            }

            if (delta == 0)
                activeCanvasIdx = 0;

            activeCanvasIdx += delta;
            activeCanvasIdx %= organizers.Length;

            organizers[activeCanvasIdx].Unfreeze();
        }

        public void UnfreezeAllTabs()
        {
            foreach (var organizer in organizers)
            {
                organizer.Unfreeze();
            }
        }

        void ItemViewClicked(ItemView itemView)
        {
            if (viewFrom == null)
            {
                if (allowSelectEmpty || itemView.GetItem() != null)
                {
                    viewFrom = itemView;
                    viewFrom.Highlight();
                }
            }
            else
            {
                InventoryItem item = viewFrom.GetItem();
                InventoryItem stashItem = itemView.GetItem();
                if (viewFrom != itemView)
                {
                    itemView.RemoveItem();
                    viewFrom.RemoveItem();

                    if (item != null)
                    {
                        itemView.PlaceItem(item);
                    }

                    if (stashItem != null)
                    {
                        viewFrom.PlaceItem(stashItem);
                    }
                }

                viewFrom.UnHighlight();
                viewFrom = null;
            }
        }
        
        void DumpToUI()
        {
            List<InventoryItem> items = GameController.Current.Inventory.InventoryItems;
        
            ItemView[] inventoryItemViews = itemContainersRoot.GetComponentsInChildren<ItemView>();
        
            int size = Mathf.Min(inventoryItemViews.Length, items.Count);
        
            for(int i = 0; i < size; i++)
                inventoryItemViews[i].PlaceItem(items[i]);
        }

        void DumpToInventory()
        {
            List<InventoryItem> items = GameController.Current.Inventory.InventoryItems;
        
            ItemView[] inventoryItemViews = itemContainersRoot.GetComponentsInChildren<ItemView>();

            int size = inventoryItemViews.Length;
            items.Clear();
        
            for (int i = 0; i < size; i++)
            {
                InventoryItem itemToAdd = inventoryItemViews[i].GetItem();
                if (itemToAdd == null)
                    continue;
                items.Add(itemToAdd);
                inventoryItemViews[i].RemoveItem();
            }
        }
    }
}