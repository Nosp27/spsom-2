using System.Collections.Generic;
using UI.Inventory.GUIMediators;
using UI.Inventory.ItemViews;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Inventory.ItemViewOrganizers
{
    public class ShipSlotCanvasOrganizer : AbstractSlotCanvasOrganizer
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

        public override void Arrange()
        {
            if (Slots.Count > 0)
                return;

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
                }
                else if (i == Slots.Count - 1)
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

        public override void Freeze()
        {
            foreach (var slot in Slots)
            {
                slot.button.interactable = false;
            }
        }

        public override void Unfreeze()
        {
            foreach (var slot in Slots)
            {
                slot.button.interactable = true;
            }
        }
    }
}