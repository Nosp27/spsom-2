using UnityEngine;

public class UIInteractionController : MonoBehaviour
{
    /*
     * This class can control item transfer between UI slots.
     * Meant to be attached to Canvas where items can be transferred between UI slots
     * Each item must have child of ItemView component
     */
    [SerializeField] private bool allowSelectEmpty;
    private ItemView viewFrom;

    public void ItemViewClicked(ItemView itemView)
    {
        if (viewFrom == null)
        {
            if (allowSelectEmpty || itemView.GetItem() != null)
            {
                viewFrom = itemView;
                viewFrom.Highlight();
            }
        } else
        {
            InventoryItem item = viewFrom.GetItem();
            if (item != null && viewFrom != itemView)
            {
                viewFrom.RemoveItem();
                itemView.PlaceItem(item);
            }
            viewFrom.UnHighlight();
            viewFrom = null;
        }
    }
}
