using UI.Inventory.ItemViewOrganizers;
using UI.Inventory.ItemViews;
using UnityEngine;
using UnityEngine.Events;

namespace UI.Inventory
{
    public class SlotGuiMediator : MonoBehaviour
    {
        [SerializeField] private AbstractSlotCanvasOrganizer[] organizers;
        [SerializeField] private InventorySyncListener listener;

        [SerializeField] private bool allowSelectEmpty;

        public UnityEvent onRunGui { get; private set; }
        public UnityEvent onStopGui { get; private set; }

        private int activeCanvasIdx;
        private ItemView viewFrom;

        private void Awake()
        {
            onRunGui = new UnityEvent();
            onStopGui = new UnityEvent();
        }

        public void Run()
        {
            listener.Run();
            foreach (AbstractSlotCanvasOrganizer organizer in organizers)
            {
                organizer.gameObject.SetActive(true);
                organizer.Arrange(this);
            }
        }

        public void Stop()
        {
            listener.End();
            foreach (AbstractSlotCanvasOrganizer organizer in organizers)
                organizer.gameObject.SetActive(false);
        }

        public void SwitchTab(int delta)
        {
            foreach (var organizer in organizers)
            {
                organizer.Freeze();
            }

            if (delta == 0)
                activeCanvasIdx = 0;

            activeCanvasIdx += delta;
            activeCanvasIdx %= organizers.Length;

            organizers[activeCanvasIdx].Unfreeze();
        }

        public void UnfreezeAllTabs()
        {
            foreach (var organizer in organizers)
            {
                organizer.Unfreeze();
            }
        }

        public void ItemViewClicked(ItemView itemView)
        {
            if (viewFrom == null)
            {
                if (allowSelectEmpty || itemView.GetItem() != null)
                {
                    viewFrom = itemView;
                    viewFrom.Highlight();
                }
            }
            else
            {
                InventoryItem item = viewFrom.GetItem();
                InventoryItem stashItem = itemView.GetItem();
                if (viewFrom != itemView)
                {
                    itemView.RemoveItem();
                    viewFrom.RemoveItem();

                    if (item != null)
                    {
                        itemView.PlaceItem(item);
                    }

                    if (stashItem != null)
                    {
                        viewFrom.PlaceItem(stashItem);
                    }
                }

                viewFrom.UnHighlight();
                viewFrom = null;
            }
        }
    }
}