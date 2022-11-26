using System;
using System.Collections.Generic;
using System.Linq;
using Unity.CodeEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    GameController gc => GameController.Current;
    [SerializeField] SlotContainer shipSelector;
    [SerializeField] SlotContainer installedPanel;
    [SerializeField] SlotContainer inventoryPanel;
    [SerializeField] private Button ConfirmButton;

    [SerializeField] private Ship ship;

    [SerializeField] GameObject[] test_AvailableShips;

    [SerializeField] private Transform ObservationPlace;

    [SerializeField] private GameObject test_PlayerShipInstance;

    public UnityAction ConfirmCallback;

    private InventorySlot ActiveInventorySlot;
    private ShipModuleSlot TargetSlot;

    private void OnEnable()
    {
        ConfirmButton.onClick.AddListener(OnConfirm);
        GameObject playerShipInstance = GameController.Current.PlayerShip.gameObject;
        if (playerShipInstance == null)
            playerShipInstance = test_PlayerShipInstance;

        ship = Instantiate(playerShipInstance, ObservationPlace.position,
            ObservationPlace.rotation, ObservationPlace).GetComponent<Ship>();
    }

    private void OnConfirm()
    {
        Ship playerShip = GameController.Current.PlayerShip;
        ship.transform.position = playerShip.transform.position;
        ship.transform.rotation = playerShip.transform.rotation;
        ship.transform.SetParent(playerShip.transform.parent);
        ship.tag = "PlayerShip";
        Destroy(playerShip.gameObject);
        GameController.Current.PlayerShip = ship;
        GameController.Current.Inventory.InventoryItems =
            inventoryPanel.Slots.Select(x => x.GetComponent<InventorySlot>().Content).Where(x => x != null).ToList();
        ConfirmCallback.Invoke();
    }

    public void SetupInventory()
    {
        List<GameObject> inventoryItems = GameController.Current.Inventory.InventoryItems;
        inventoryPanel.OnSelect.AddListener(() => SelectEventCallback(inventoryPanel));
        inventoryPanel.CreateGrid(GameController.Current.Inventory.maxSize);

        for (int i = 0; i < inventoryItems.Count; i++)
        {
            InventorySlot _slot = inventoryPanel.Slots[i] as InventorySlot;
            _slot.SetActionText("INSTALL");
            _slot.PutItem(inventoryItems[i]);
        }
    }

    public void SetupInstalled()
    {
        ModulePylon[] pylons = ship.GetComponentsInChildren<ModulePylon>();

        if (pylons.Length == 0)
            return;

        Vector3[] positions = new Vector3[pylons.Length];

        for (int i = 0; i < positions.Length; i++)
        {
            positions[i] = Quaternion.AngleAxis(-90, Vector3.right) * ship.transform.InverseTransformVector(
                Vector3.ProjectOnPlane(
                    pylons[i].transform.position - ship.transform.position,
                    ship.transform.up
                )
            );
        }

        installedPanel.CreateFromWorldPoints(positions);
        installedPanel.OnSelect.AddListener(() => SelectEventCallback(installedPanel));

        for (int i = 0; i < pylons.Length; i++)
        {
            installedPanel.Slots[i].GetComponent<ShipModuleSlot>().AttachPylon(pylons[i]);
        }
    }

    public void SetupShipSelector()
    {
        return;
        shipSelector.CreateList(test_AvailableShips.Length);

        shipSelector.OnSelect.AddListener(() => ShipSelectorEventCallback(shipSelector));
        for (int i = 0; i < test_AvailableShips.Length; i++)
        {
            shipSelector.Slots[i].GetComponentInChildren<Text>().text = test_AvailableShips[i].name;
            shipSelector.Slots[i].GetComponentInChildren<InventorySlot>().Content = test_AvailableShips[i];
        }
    }

    void ShipSelectorEventCallback(SlotContainer source)
    {
        if (source != shipSelector)
            return;

        GameObject newShipPrefab = (source.Selected as InventorySlot)?.Content;

        if (newShipPrefab)
        {
            GameObject newShip = Instantiate(newShipPrefab);
            newShip.transform.position = ObservationPlace.transform.position;
            newShip.transform.rotation = ObservationPlace.transform.rotation;
            Destroy(ship.gameObject);
            ship = newShip.GetComponent<Ship>();

            SetupInventory();
            SetupInstalled();
        }
    }

    void SelectEventCallback(SlotContainer source)
    {
        if (source == inventoryPanel)
        {
            ActiveInventorySlot = source.Selected as InventorySlot;
        }

        if (source == installedPanel)
        {
            TargetSlot = source.Selected as ShipModuleSlot;
        }

        if (!(ActiveInventorySlot && TargetSlot))
            return;

        if (
            ActiveInventorySlot.Content && !TargetSlot.Engaged
        )
        {
            // Engage module from inventory
            if (TargetSlot.Install(ActiveInventorySlot.Content.GetComponent<ShipModule>()))
                inventoryPanel.Selected.GetComponent<InventorySlot>().PutItem(null);
        }
        else if (!ActiveInventorySlot.Content && TargetSlot.Engaged)
        {
            // Disengage module and put to inventory
            ActiveInventorySlot.PutItem(TargetSlot.ModulePrefab);
            TargetSlot.Uninstall();
        }
        else if (ActiveInventorySlot.Content && TargetSlot.Engaged)
        {
            GameObject prevInstalledModule = TargetSlot.ModulePrefab;
            TargetSlot.Uninstall();
            if (TargetSlot.Install(ActiveInventorySlot.Content.GetComponent<ShipModule>()))
            {
                inventoryPanel.Selected.GetComponent<InventorySlot>().PutItem(prevInstalledModule);
            }
        }


        ActiveInventorySlot = null;
        TargetSlot = null;
        installedPanel.DropSelection();
        inventoryPanel.DropSelection();
    }
}