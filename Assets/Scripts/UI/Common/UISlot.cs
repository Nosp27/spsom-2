using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    public virtual void Highlight(bool enable)
    {
        Outline outline = GetComponent<Outline>();
        if (outline)
            outline.enabled = enable;
    }

    public virtual void AddAction(UnityAction a)
    {
        GetComponentInChildren<Button>().onClick.AddListener(a);
    }
}
