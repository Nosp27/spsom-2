using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class UIControllerShipModules : MonoBehaviour
{
    /*
     * UI controller that is responsible for placing ship modules. Arranges and controls WorldBoundItemView objects.
     */
    private Ship attachedShip;
    [SerializeField] private GameObject SlotPrefab;
    private List<WorldBoundItemView> Slots;

    private void Awake()
    {
        Slots = new List<WorldBoundItemView>();
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
        
        Slots.Sort((x, y) => x.transform.localPosition.x.CompareTo(y.transform.localPosition.x));
        for (int i = 0; i < Slots.Count; i++)
        {
            Button left;
            Button right;
            if (i == 0)
            {
                left = Slots[Slots.Count - 1].button;
                right = Slots[1].button;
            } else if (i == Slots.Count - 1)
            {
                left = Slots[Slots.Count - 2].button;
                right = Slots[0].button;
            }
            else
            {
                left = Slots[i - 1].button;
                right = Slots[i + 1].button;
            }
            Button btn = Slots[i].button;
            var nav = btn.navigation;
            nav.mode = Navigation.Mode.Explicit;
            nav.selectOnLeft = left;
            nav.selectOnRight = right;
            btn.navigation = nav;
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
