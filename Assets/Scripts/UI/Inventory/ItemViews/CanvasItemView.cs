using UnityEngine;
using UnityEngine.UI;

public class CanvasItemView : ItemView
{
    /*
     * Item view for UI element on Canvas. For example a UI grid.
     *
     * Check out parent docstring
     */
    private Image itemImageComponent;
    private Color baseColor;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    void OnEnable()
    {
        itemImageComponent = transform.GetChild(0).GetComponent<Image>();
        itemImageComponent.enabled = false;
        itemContainer = GetComponent<ItemContainer>();
        m_UIInteractionController = GetComponentInParent<UIInteractionController>();
        baseColor = GetComponent<Image>().color;
    }

    public override void Highlight()
    {
        print("H");
        GetComponent<Image>().color = Color.green;
    }
    
    public override void UnHighlight()
    {
        print("UH");
        GetComponent<Image>().color = baseColor;
    }

    public override void PlaceItem(InventoryItem item)
    {
        Sprite itemSprite = item.GetComponent<InventoryItem>().Icon;
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
