using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIControllerInventory : MonoBehaviour
{
    /*
     * This class can control inventory UI.
     * It connects to `InventoryController`, does fetch items to corresponding ui places, tracks changes and
     * updates Player inventory
     *
     * Meant to be attached to Inventory Canvas
     */
    [SerializeField] private GameObject InventoryItemsPanel;

    private HashSet<Selectable> world;
    private HashSet<Selectable> ui;
    private bool isWorld;

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

        UIControllerShipModules shipModulesController = GetShipModulesController();
        shipModulesController.gameObject.SetActive(true);
        if (shipModulesController)
            shipModulesController.BindInteractionController(GetComponent<UIInteractionController>());
        SetupButtonTracking();
    }

    void SetupButtonTracking()
    {
        ui = new HashSet<Selectable>(InventoryItemsPanel.GetComponentsInChildren<Button>(true));
        world = new HashSet<Selectable>(GetShipModulesController().GetComponentsInChildren<Button>(true));
        SetCanvasInteractibility(isWorld);
    }

    private void Update()
    {
        if (Gamepad.current != null && (Gamepad.current.rightShoulder.wasPressedThisFrame || Gamepad.current.leftShoulder.wasPressedThisFrame))
        {
            isWorld = !isWorld;
            SetCanvasInteractibility(isWorld);
        }
    }

    void SetCanvasInteractibility(bool isWorld)
    {
        foreach (var b in ui)
        {
            b.interactable = !isWorld;
        }
        foreach (var b in world)
        {
            b.interactable = isWorld;
        }
        if (isWorld)
        {
            world.First().Select();
        }
        else {
            ui.First().Select();
        }
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
        UIControllerShipModules shipModulesController = GetShipModulesController();
        shipModulesController.gameObject.SetActive(false);
    }

    UIControllerShipModules GetShipModulesController()
    {
        return GameController.Current.PlayerShip?.GetComponentInChildren<UIControllerShipModules>(true);
    }
}