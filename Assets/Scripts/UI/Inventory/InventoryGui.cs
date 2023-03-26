using System.Collections.Generic;
using ShipModules;
using UI.Inventory.SlotCanvas;
using UnityEngine;

namespace UI.Inventory
{
    public class InventoryGui : MonoBehaviour
    {
        private CanvasGroup cg;

        private InventoryController inventoryController => GameController.Current.Inventory;
        private Ship player => GameController.Current.PlayerShip;
        private ModulePylon[] shipPylons => player.GetComponentsInChildren<ModulePylon>();
        private int numPylons => shipPylons.Length;

        [SerializeField] private CircularCanvasInput inputController;

        private Dictionary<ItemView, ModulePylon> itemModuleMapping;

        private bool isRunning;

        private void Awake()
        {
            itemModuleMapping = new Dictionary<ItemView, ModulePylon>();
        }

        public void Start()
        {
            cg = GetComponent<CanvasGroup>();
            
            inputController.Init();
            inputController.onClick.AddListener(Click);
            foreach (var iv in inputController.itemViews)
            {
                iv.UnHighlight();
            }
            
            inventoryController.onPutItem.AddListener(AddItem);
            inventoryController.onRemoveItem.AddListener(RemoveItem);
        }

        public void Run()
        {
            isRunning = true;
            cg.alpha = 1;
            cg.interactable = true;
            cg.blocksRaycasts = true;
        }

        public void Stop()
        {
            if (cg == null)
                cg = GetComponent<CanvasGroup>();
            isRunning = false;
            cg.alpha = 0;
            cg.interactable = false;
            cg.blocksRaycasts = false;
        }

        private void AddItem(InventoryItem item)
        {
            foreach (ItemView itemView in inputController.itemViews)
            {
                if (itemView.content != null)
                    continue;
                itemView.content = item;
                break;
            }
        }

        private bool IsInstalled(ItemView itemView)
        {
            return itemModuleMapping.ContainsKey(itemView);
        }
        
        private void RemoveItem(InventoryItem item)
        {
            foreach (ItemView itemView in inputController.itemViews)
            {
                if (itemView.content != item)
                    continue;

                if (IsInstalled(itemView))
                {
                    UninstallItem(itemView);
                }
                
                itemView.content = null;
                return;
            }
        }

        private void UninstallItem(ItemView itemView)
        {
            GameObject module = itemModuleMapping[itemView].Uninstall();
            module.transform.SetParent(inventoryController.transform);
            module.transform.localPosition = Vector3.zero;
            itemModuleMapping.Remove(itemView);
            itemView.UnHighlight();
        }

        private void Click(ItemView itemView)
        {
            if (!isRunning)
                return;
            if (itemView.content == null)
                return;
            
            if (IsInstalled(itemView))
            {
                UninstallItem(itemView);
            }
            else
            {
                // Try to install
                foreach (ModulePylon pylon in shipPylons)
                {
                    if (pylon.content == null)
                    {
                        pylon.Install(itemView.content.gameObject);
                        itemView.Highlight();
                        itemModuleMapping[itemView] = pylon;
                        break;
                    }
                }
            }
        }
    }
}