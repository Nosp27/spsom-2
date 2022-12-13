using System;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    private GameObject item;

    void Start()
    {
        
    }

    public GameObject GetItem()
    {
        return item;
    }
    
    void SetItem(GameObject newItem)
    {
        item = newItem;
    }

    public bool ChangeItem(GameObject newItem)
    {
        if (newItem == null)
        {
            throw new Exception("Try to change item to null");
        }
        
        if (item == null)
        {
            SetItem(newItem);
            item = newItem;
            return true;
        }

        return false;
    }

    public bool DropItem()
    {
        if (item == null)
            return false;

        SetItem(null);
        return true;
    }
}
