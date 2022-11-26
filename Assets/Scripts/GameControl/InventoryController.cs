using System.Collections.Generic;
using UnityEngine;


public class InventoryController : MonoBehaviour
{
    public int maxSize = 9;
    public List<GameObject> InventoryItems;

    private void Awake()
    {
        if (InventoryItems == null)
            InventoryItems = new List<GameObject>(maxSize);
    }

    public void PutItem(GameObject item)
    {
        InventoryItems.Add(item);
    }

    public void RemoveItem(GameObject item)
    {
        InventoryItems.Remove(item);
    }
}
