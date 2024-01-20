using System.Collections.Generic;
using GameEventSystem;
using UI.Inventory;
using UnityEngine;


public class InventoryController : MonoBehaviour
{
    /*
     * Game control related class. Controls Player Inventory.
     *
     * Now it also does control appearence of Inventory UI and pausing the game, but it has to be replaced somewhere
     * else
     *
     * Attach to GameController dummy or to its child (for better structure)
     */

    public int maxSize = 8;
    public List<InventoryItem> InventoryItems;

    private void Awake()
    {
        if (InventoryItems == null)
            InventoryItems = new List<InventoryItem>(maxSize);
    }

    public void PutItem(InventoryItem item)
    {
        print("item added");
        InventoryItems.Add(item);
        EventLibrary.onInventoryPutItem.Invoke(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        InventoryItems.Remove(item);
        EventLibrary.onInventoryrRemoveItem.Invoke(item);
    }
}