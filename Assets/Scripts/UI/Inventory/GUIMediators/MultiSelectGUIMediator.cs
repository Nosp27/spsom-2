using System.Collections.Generic;
using System.Linq;
using UI.Inventory.ItemViewOrganizers;
using UI.Inventory.ItemViews;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Inventory.GUIMediators
{
    public class MultiSelectGUIMediator : MonoBehaviour
    {
        [SerializeField] private AbstractSlotCanvasOrganizer organizer;

        private Dictionary<int, ItemView> installedItems;
        private Dictionary<ItemView, int> uiItems;

        public UnityEvent onRunGui { get; private set; }
        public UnityEvent onStopGui { get; private set; }

        private void Awake()
        {
            installedItems = new Dictionary<int, ItemView>();
            uiItems = new Dictionary<ItemView, int>();
            onRunGui = new UnityEvent();
            onStopGui = new UnityEvent();
        }

        public void Run()
        {
            organizer.gameObject.SetActive(true);
            DumpToUI();
            organizer.Arrange();
            organizer.SetClickListener(ItemViewClicked);
        }

        public void Stop()
        {
            DumpToInventory();
            organizer.gameObject.SetActive(false);
        }

        void ItemViewClicked(ItemView itemView)
        {
            if (installedItems.Contains(itemView))
                installedItems.Remove(itemView);
            else
                installedItems.Add(itemView);
        }

        void LinkUiItems()
        {
            
        }
        
        void DumpToUI()
        {
            List<InventoryItem> items = GameController.Current.Inventory.InventoryItems;

            // int size = Mathf.Min(inventoryItemViews.Length, items.Count);
            int size = items.Count;
        
            for(int i = 0; i < size; i++)
            {
                if (installedItems.ContainsKey(i))
                    continue;
                uiItems[i] = items[i];
                inventoryItemViews[i].PlaceItem(items[i]);
            }
        }
        
        void DumpToInventory()
        {
            List<InventoryItem> items = GameController.Current.Inventory.InventoryItems;
        
            ItemView[] inventoryItemViews = GetComponentsInChildren<ItemView>();

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