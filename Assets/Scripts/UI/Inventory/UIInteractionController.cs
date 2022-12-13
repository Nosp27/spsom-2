using UnityEngine;

public class UIInteractionController : MonoBehaviour
{
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
