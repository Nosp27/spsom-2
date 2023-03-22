using UnityEngine;

namespace UI.Inventory.ItemViewOrganizers
{
    public abstract class AbstractSlotCanvasOrganizer : MonoBehaviour
    {
        public abstract void Arrange(SlotGuiMediator mediator);
        public abstract void Freeze();
        public abstract void Unfreeze();
    }
}