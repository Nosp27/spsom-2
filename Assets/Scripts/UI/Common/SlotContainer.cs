using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SlotContainer : MonoBehaviour
{
    [SerializeField] public GameObject SlotPrefab;
    [NonSerialized] public UnityEvent OnSelect;

    public UISlot[] Slots;
    public UISlot Selected { get; private set; }

    private void Awake()
    {
        OnSelect = new UnityEvent();
    }

    private void DeleteOldSlots()
    {
        if (Slots != null)
        {
            foreach (var slot in Slots)
            {
                Destroy(slot.gameObject);
            }
        }

        Slots = null;
    }

    public void CreateFromWorldPoints(Vector3[] worldPoints)
    {
        DeleteOldSlots();
        
        if (worldPoints == null || worldPoints.Length == 0)
            return;
        
        RectTransform slotsPanel = GetComponent<RectTransform>();

        Slots = new UISlot[worldPoints.Length];

        float maxDistance = worldPoints.Select(x => x.magnitude).Max();
        float distanceLimit = slotsPanel.rect.height / 2;
        float multiplier = distanceLimit / (maxDistance + 1);

        int i = 0;
        foreach (var position in worldPoints.Select(x => x * multiplier).ToArray())
        {
            GameObject uiItem = Instantiate(SlotPrefab, slotsPanel);
            uiItem.transform.localPosition = position;
            Slots[i] = uiItem.GetComponent<UISlot>();
            TrackSelection(Slots[i]);
            i++;
        }
    }

    public void CreateGrid(int n)
    {
        RectTransform slotsPanel = GetComponent<RectTransform>();
        DeleteOldSlots();

        Slots = new UISlot[n];
        int nSlots = n;
        int maxHeight = Mathf.CeilToInt(Mathf.Sqrt(nSlots));
        int maxWidth = Mathf.CeilToInt(1f * nSlots / maxHeight);
        RectTransform rt = slotsPanel;
        Rect rect = rt.rect;

        Vector3 initialPoint = - Vector3.right * rect.width/2 + (Vector3.up * rect.height/2);
        
        float slotWspan = (rect.width / maxWidth) / 8;
        float slotHspan = (rect.height / maxHeight) / 8;

        Vector3 fixedOffset = new Vector3(slotWspan, -slotHspan, 0);

        float slotW = (rect.width - (2 + maxWidth - 1) * slotWspan) / maxWidth;
        float slotH = (rect.height - (2 + maxHeight - 1) * slotHspan) / maxHeight;
        
        for (int i = 0; i < n; i++)
        {
            GameObject uiItem = Instantiate(SlotPrefab, slotsPanel);
            Slots[i] = uiItem.GetComponent<UISlot>();

            TrackSelection(Slots[i]);
            int col = (i % maxWidth);
            int row = (int) (i / maxWidth);
            
            Vector3 positionOffset = new Vector3(
                col * (slotW + slotWspan),
                -row * (slotH + slotHspan),
                0
            );

            RectTransform slotRectTransform = Slots[i].GetComponent<RectTransform>();
            slotRectTransform.localPosition = initialPoint + fixedOffset + positionOffset;

            slotRectTransform.sizeDelta = new Vector2(slotW,slotH);
        }
    }
    
    public void CreateList(int n)
    {
        RectTransform slotsPanel = GetComponent<RectTransform>();
        DeleteOldSlots();

        Slots = new UISlot[n];
        int verticalStep = (int)(SlotPrefab.GetComponent<RectTransform>().rect.height * 1.2f);
        RectTransform rt = slotsPanel;

        Vector3 fixedOffset = new Vector3(0, -(rt.rect.height / verticalStep / 2), 0);

        for (int i = 0; i < n; i++)
        {
            GameObject uiItem = Instantiate(SlotPrefab, slotsPanel);
            Slots[i] = uiItem.GetComponent<UISlot>();

            TrackSelection(Slots[i]);

            Rect rect = rt.rect;
            Vector3 positionOffset = new Vector3(0, -i * verticalStep, 0);
            Vector3 initialPoint = rt.position + Vector3.up * (rect.height / 2 - verticalStep / 2);
            Slots[i].GetComponent<RectTransform>().position = initialPoint + fixedOffset + positionOffset;
        }
    }

    public void DropSelection()
    {
        if (Selected == null)
            return;
        Selected.SendMessage("Highlight", false);
        Selected = null;
    }

    void TrackSelection(UISlot s)
    {
        s.AddAction(() =>
        {
            if (Selected != s)
            {
                DropSelection();
                s.Highlight(true);
                Selected = s;
            }
            else
            {
                DropSelection();
            }
            OnSelect.Invoke();
        });
    }
}