using System.Collections.Generic;
using UnityEngine;

public class UIControllerShipModules : MonoBehaviour
{
    /*
     * UI controller that is responsible for placing ship modules. Arranges and controls WorldBoundItemView objects.
     */
    private Ship attachedShip;
    [SerializeField] private GameObject SlotPrefab;
    private List<ItemView> Slots;

    private void Awake()
    {
        Slots = new List<ItemView>();
        gameObject.SetActive(false);
    }

    void ArrangeShipModules()
    {
        attachedShip = GetComponentInParent<Ship>();
        foreach (ModulePylon pylon in attachedShip.GetComponentsInChildren<ModulePylon>())
        {
            GameObject worldUiSlotPrefab = Instantiate(SlotPrefab, transform);
            WorldBoundItemView itemView = worldUiSlotPrefab.GetComponent<WorldBoundItemView>();
            itemView.BindReference(pylon.gameObject);
            Slots.Add(itemView);
        }
    }

    public void BindInteractionController(UIInteractionController controller)
    {
        if (Slots.Count == 0)
            ArrangeShipModules();
        
        foreach (WorldBoundItemView view in Slots)
        {
            view.BindController(controller);
        }
    }
}
