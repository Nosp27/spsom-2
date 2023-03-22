using System.Collections.Generic;
using UI.Inventory.ItemViews;
using UnityEngine;

namespace UI.Inventory
{
    public class InventorySyncListener : AbstractSlotGuiListener
    {
        // Class is responsible for synchronizing items between collection of ItemContainer-s and Inventory

        [SerializeField] private Transform itemContainersRoot;

        public override void Run()
        {
            DumpToUI();
        }

        public override void End()
        {
            DumpToInventory();
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
