using UnityEngine;
using Random = UnityEngine.Random;

public class Loot : MonoBehaviour
{
    /// <summary>
    /// Attach component to loot item that floats in space. It has data for inventory system to create inventoryItem
    /// and use it.
    /// </summary>
    
    [SerializeField] private GameObject[] lootInventoryItemPrefabs;

    private int prefabIndex;
    
    public void Awake()
    {
        prefabIndex = Random.Range(0, lootInventoryItemPrefabs.Length);
    }

    public GameObject GetLootPrefab()
    {
        return lootInventoryItemPrefabs[prefabIndex];
    }
}
