using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class InventorySlot : UISlot
{
    public GameObject Content;
    public Image SpritePlace;

    public void SetActionText(string text)
    {
        
    }

    public void PutItem([CanBeNull]GameObject item)
    {
        if (item == null)
        {
            SpritePlace.sprite = null;
            SpritePlace.enabled = false;
            Content = null;
            return;
        }

        Content = item;
        SpritePlace.sprite = Content.GetComponent<InventoryItem>().Icon;
        SpritePlace.enabled = true;
    }

    public void AddClickListener(UnityAction e)
    {
        GetComponent<Button>().onClick.AddListener(e);
    }
}
