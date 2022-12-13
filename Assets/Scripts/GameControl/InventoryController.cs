using System.Collections.Generic;
using UnityEngine;


public class InventoryController : MonoBehaviour
{
    /*
     * Game control related class. Controls Player Inventory.
     *
     * Now it also does control appearence of Inventory UI and pausing the game, but it has to be replaced somewhere
     * else
     *
     * Attach to GameController dummy or to its child (for better structure)
     */
    private bool IsInInventoryMode;
    
    public int maxSize = 9;
    public List<InventoryItem> InventoryItems;
    [SerializeField] private GameObject InventoryCanvas;

    private void Awake()
    {
        if (InventoryItems == null)
            InventoryItems = new List<InventoryItem>(maxSize);
        InventoryCanvas.SetActive(false);
    }

    public void PutItem(InventoryItem item)
    {
        InventoryItems.Add(item);
    }

    public void RemoveItem(InventoryItem item)
    {
        InventoryItems.Remove(item);
    }

    // TODO: Remove after Debug
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchInventoryMode();
        }
    }

    void SwitchInventoryMode()
    {
        bool newInventoryMode = !IsInInventoryMode;
        GameController.Current.SwitchCursorControl(!newInventoryMode);
        InventoryCanvas.SetActive(newInventoryMode);
        Cursor.visible = newInventoryMode;
        Time.timeScale = newInventoryMode ? 0 : 1;
        IsInInventoryMode = newInventoryMode;
    }
}
