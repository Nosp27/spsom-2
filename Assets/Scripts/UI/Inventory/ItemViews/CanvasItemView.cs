using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory.ItemViews
{
    public class CanvasItemView : ItemView
    {
        /*
         * Item view for UI element on Canvas. For example a UI grid.
         *
         * Check out parent docstring
         */
        private bool initDone = false;
        private Image itemImageComponent;
        private Color baseColor;

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        void OnEnable()
        {
            if (initDone)
                return;
            initDone = true;
            itemImageComponent = transform.GetChild(0).GetComponent<Image>();
            itemImageComponent.enabled = false;
            itemContainer = GetComponent<ItemContainer>();
            baseColor = GetComponent<Image>().color;
        }

        private void OnDisable()
        {
            initDone = false;
        }

        public override void Highlight()
        {
            GetComponent<Image>().color = Color.green;
        }

        public override void UnHighlight()
        {
            GetComponent<Image>().color = baseColor;
        }

        public override void PlaceItem(InventoryItem item)
        {
            OnEnable();
            Sprite itemSprite = item.Icon;
            itemImageComponent.sprite = itemSprite;
            itemImageComponent.enabled = true;
            itemContainer.DropItem();
            itemContainer.ChangeItem(item.gameObject);
        }

        public override void RemoveItem()
        {
            itemImageComponent.sprite = null;
            itemImageComponent.enabled = false;
            itemContainer.DropItem();
        }
    }
}