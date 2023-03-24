using System;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Inventory.ItemViews
{
    public class ItemView : MonoBehaviour
    {
        /*
     * Controls UI slot that can have an item in it.
     * Item can be replaced, dropped or retrieved by other script.
     * Items must be instantiated (do not use prefabs as items)
     *
     * Requires ItemContainer that controls Item binding to cargo place.
     *
     * Class is not meant to be attached by itself. Its children must be used
     *
     * Attach child to UI element
     */
        protected ItemContainer itemContainer;
        public UnityEvent<ItemView> onClick;

        bool _lazyInitDone;

        private void Awake()
        {
            onClick = new UnityEvent<ItemView>();
        }

        protected void LazyInit()
        {
            if (_lazyInitDone)
                return;
            _lazyInitDone = true;
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
            onClick.Invoke(this);
        }

        public virtual InventoryItem GetItem()
        {
            LazyInit();
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
}
