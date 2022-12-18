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
            InventoryItem stashItem = itemView.GetItem();
            if (viewFrom != itemView)
            {
                itemView.RemoveItem();
                viewFrom.RemoveItem();
                
                if (item != null)
                {
                    itemView.PlaceItem(item);
                }

                if (stashItem != null)
                {
                    viewFrom.PlaceItem(stashItem);
                }
            }
            viewFrom.UnHighlight();
            viewFrom = null;
        }
    }
}
