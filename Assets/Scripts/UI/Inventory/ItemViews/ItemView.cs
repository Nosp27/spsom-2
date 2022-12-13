using System;
using UnityEngine;

public class ItemView : MonoBehaviour
{
    protected ItemContainer itemContainer;
    protected UIInteractionController m_UIInteractionController;

    protected virtual void Start()
    {
        itemContainer = GetComponent<ItemContainer>();
    }
    
    public virtual void Highlight()
    {
        throw new NotImplementedException();
    }
    
    public virtual void UnHighlight()
    {
        throw new NotImplementedException();
    }
    
    protected void OnClick()
    {
        if (m_UIInteractionController)
            m_UIInteractionController.ItemViewClicked(this);
        else
        {
            Debug.LogError("No Interaction Controller attached");
        }
    }

    public InventoryItem GetItem()
    {
        return itemContainer.GetItem()?.GetComponent<InventoryItem>();
    }

    public virtual void PlaceItem(InventoryItem item)
    {
        throw new NotImplementedException();
    }

    public virtual void RemoveItem()
    {
        throw new NotImplementedException();
    }
}
