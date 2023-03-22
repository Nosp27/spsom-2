using UI.Inventory.ItemViews;
using UnityEngine.UI;

namespace UI.Inventory.ItemViewOrganizers
{
    public class InventoryCanvasOrganizer : AbstractSlotCanvasOrganizer
    {
        public override void Arrange(SlotGuiMediator mediator)
        {
            foreach (var itemView in GetComponentsInChildren<ItemView>())
            {
                itemView.onClick.AddListener(mediator.ItemViewClicked);
            }
        }

        public override void Freeze()
        {
            print($"Freeze {name}");
            foreach (var itemView in GetComponentsInChildren<ItemView>())
            {
                itemView.GetComponent<Button>().interactable = false;
            }
        }

        public override void Unfreeze()
        {
            print($"Unfreeze {name}");
            foreach (var itemView in GetComponentsInChildren<ItemView>())
            {
                itemView.GetComponent<Button>().interactable = true;
            }
        }
    }
}