using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Inventory
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private float colorTransitionTime = 0.1f;
        [SerializeField] private Color initialColor = Color.white;
        [SerializeField] private Color highlightedColor = Color.green;
        [SerializeField] private Color hoveredColor = Color.yellow;
        [SerializeField] private Color hoveredHighlightedColor = Color.green + Color.yellow;

        [SerializeField] private Image itemImage;

        private Button button;

        [HideInInspector] public bool isHighlighted;
        private bool isHovered;
        private Tween colorTween;

        private InventoryItem _content;
        
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
            ChangeColor(isHovered ? hoveredHighlightedColor : highlightedColor);
        }

        public void UnHighlight()
        {
            isHighlighted = false;
            ChangeColor(isHovered ? hoveredColor : initialColor);
        }

        public void Hover()
        {
            isHovered = true;
            ChangeColor(isHighlighted ? hoveredHighlightedColor : hoveredColor);
        }

        public void UnHover()
        {
            isHovered = false;
            ChangeColor(isHighlighted ? highlightedColor : initialColor);
        }

        private void ChangeColor(Color targetColor)
        {
            button.image.color = targetColor;
            // colorTween?.Kill();
            // colorTween = button.image.DOBlendableColor(targetColor, colorTransitionTime);
        }
    }
}