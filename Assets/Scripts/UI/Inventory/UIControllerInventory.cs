using System.Collections.Generic;
using UnityEngine;

public class UIControllerInventory : MonoBehaviour
{
    [SerializeField] private GameObject InventoryItemsPanel;

    void OnEnable()
    {
        if (!GameController.Current?.Inventory)
            return;
        
        List<InventoryItem> items = GameController.Current.Inventory.InventoryItems;
        CanvasItemView[] inventoryItemViews = GetComponentsInChildren<CanvasItemView>();
        int size = Mathf.Min(inventoryItemViews.Length, items.Count);
        for (int i = 0; i < size; i++)
        {
            inventoryItemViews[i].PlaceItem(items[i]);
        }

        UIControllerShipModules shipModulesController =
            GameController.Current.PlayerShip?.GetComponentInChildren<UIControllerShipModules>(true);

        shipModulesController.gameObject.SetActive(true);
        if (shipModulesController)
            shipModulesController.BindInteractionController(GetComponent<UIInteractionController>());
    }

    private void OnDisable()
    {
        if (!GameController.Current?.Inventory)
            return;   
        List<InventoryItem> items = GameController.Current.Inventory.InventoryItems;
        CanvasItemView[] inventoryItemViews = GetComponentsInChildren<CanvasItemView>();
        items.Clear();
        foreach (CanvasItemView i in inventoryItemViews)
        {
            InventoryItem it = i.GetItem();
            if (it)
            {
                items.Add(it);
                i.RemoveItem();
            }
        }
    }
}