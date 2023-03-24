using UI.Inventory.GUIMediators;
using UI.Inventory.ItemViews;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UI.Inventory.ItemViewOrganizers
{
    public class InventoryCanvasOrganizer : AbstractSlotCanvasOrganizer
    {
        public override void Arrange()
        {
        }

        public override void Freeze()
        {
            foreach (var itemView in GetComponentsInChildren<ItemView>())
            {
                itemView.GetComponent<Button>().interactable = false;
            }
        }

        public override void Unfreeze()
        {
            foreach (var itemView in GetComponentsInChildren<ItemView>())
            {
                itemView.GetComponent<Button>().interactable = true;
            }
        }
    }
}