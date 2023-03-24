using UI.Inventory.ItemViews;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Inventory.ItemViewOrganizers
{
    public abstract class AbstractSlotCanvasOrganizer : MonoBehaviour
    {
        public abstract void Arrange();
        public abstract void Freeze();
        public abstract void Unfreeze();

        public void SetClickListener(UnityAction<ItemView> clickAction)
        {
            foreach (ItemView iv in GetComponentsInChildren<ItemView>())
            {
                iv.onClick.RemoveAllListeners();
                iv.onClick.AddListener(clickAction);
            }
        }
    }
}