using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;

    [SerializeField] private GameObject slotPrefab;

    private void OnEnable()
    {
        List<GameObject> inventoryItems = GameController.Current.Inventory.InventoryItems;

        foreach (GameObject item in inventoryItems)
        {
            InventorySlot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<InventorySlot>();
            slot.Content = item;
            slot.AddClickListener(() =>
            {
                
            });
        }
    }
}