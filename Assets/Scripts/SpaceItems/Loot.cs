using System;
using UnityEngine;
using UnityEngine.Events;

public class Loot : MonoBehaviour
{
    /// <summary>
    /// Attach component to loot item that floats in space. It has data for inventory system to create inventoryItem
    /// and use it.
    /// </summary>
    
    [SerializeField] private GameObject lootInventoryItemPrefab;

    public GameObject GetLootPrefab()
    {
        return lootInventoryItemPrefab;
    }
}
