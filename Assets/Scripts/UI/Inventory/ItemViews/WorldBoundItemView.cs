using UnityEngine;
using UnityEngine.UI;

public class WorldBoundItemView : ItemView
{
    /*
     * Item view for UI element that is attached to some slot in world space (e.g Ship module)
     *
     * Check out parent docstring
     */
    private GameObject referenceSlot;
    [SerializeField] private GameObject hoverMarker;

    public Button button { get; private set; }


    void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void Hover(bool activate)
    {
        if (hoverMarker)
            hoverMarker.SetActive(activate);
    }
    
    
    
    public override void Highlight()
    {
    }

    public override void UnHighlight()
    {
    }

    public void BindReference(GameObject referenceSlot)
    {
        this.referenceSlot = referenceSlot;
        transform.position = referenceSlot.transform.position;
        Vector3 lp = transform.localPosition;
        lp.z = 0;
        transform.localPosition = lp;
    }

    public void BindController(UIInteractionController controller)
    {
        m_UIInteractionController = controller;
    }

    public override void PlaceItem(InventoryItem item)
    {
        item.transform.position = referenceSlot.transform.position;
        item.transform.rotation = referenceSlot.transform.rotation;
        item.transform.SetParent(referenceSlot.transform);
        item.gameObject.SetActive(true);

        itemContainer.DropItem();
        itemContainer.ChangeItem(item.gameObject);
    }

    public override void RemoveItem()
    {
        GameObject item = itemContainer.GetItem();
        if (item == null)
            return;
        itemContainer.DropItem();
        item.gameObject.transform.position = Vector3.zero;
        item.transform.SetParent(GameController.Current.Inventory.transform);
        item.gameObject.SetActive(false);
    }
}