using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory.ItemViews
{
    public class SelectableCanvasItemView : CanvasItemView
    {
        /*
         * Same as canvas item view but supports state
         *
         * Check out parent docstring
         */
        
        // Occupied means taken from UI to some world stuff. Then is has to be considered empty
        [SerializeField] private bool inverseOccupied = true;
        
        public bool occupied { get; private set; }

        public override void Highlight()
        {
            base.Highlight();
            occupied = true;
        }

        public override void UnHighlight()
        {
            base.UnHighlight();
            occupied = false;
        }

        public InventoryItem ItemForUI()
        {
            return base.GetItem();
        }

        public override InventoryItem GetItem()
        {
            if (occupied == inverseOccupied)
                return null;
            return base.GetItem();
        }
    }
}