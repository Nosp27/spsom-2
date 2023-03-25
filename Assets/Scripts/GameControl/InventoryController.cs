using System;
using System.Collections.Generic;
using UI.Inventory;
using UnityEngine;
using UnityEngine.Events;


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

    public UnityEvent<InventoryItem> onPutItem { get; private set; }
    public UnityEvent<InventoryItem> onRemoveItem { get; private set; }

    private void Awake()
    {
        onPutItem = new UnityEvent<InventoryItem>();
        onRemoveItem = new UnityEvent<InventoryItem>();

        if (InventoryItems == null)
            InventoryItems = new List<InventoryItem>(maxSize);
    }

    public void PutItem(InventoryItem item)
    {
        print("item added");
        InventoryItems.Add(item);
        onPutItem.Invoke(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        InventoryItems.Remove(item);
        onRemoveItem.Invoke(item);
    }
}