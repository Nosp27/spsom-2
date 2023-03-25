using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Image itemImage;
        
        [HideInInspector] private Button button;
        [HideInInspector] public bool isHighlighted;

        private InventoryItem _content;

        [HideInInspector]
        public InventoryItem content
        {
            get => _content;
            set
            {
                itemImage.sprite = value.Icon;
                _content = value;
            }
        }

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void Highlight()
        {
            isHighlighted = true;
            button.image.color = Color.green;
        }

        public void UnHighlight()
        {
            isHighlighted = false;
            button.image.color = Color.white;
        }
    }
}