using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Inventory.SlotCanvas
{
    public class CircularCanvasInput : MonoBehaviour
    {
        public ItemView[] itemViews { get; private set; }
        private Vector2[] buttonDirections;
        private int currentHighlighted;

        // Only for visuals - not to select same button every frame
        private ItemView lastHover;

        public UnityEvent<ItemView> onClick { get; private set; }

        public void Init()
        {
            onClick = new UnityEvent<ItemView>();
            Vector3 center = transform.position;
            itemViews = GetComponentsInChildren<ItemView>();
            buttonDirections = new Vector2[itemViews.Length];

            int i = 0;
            foreach (ItemView itemView in itemViews)
            {
                buttonDirections[i++] = ((Vector2) (itemView.transform.position - center)).normalized;
                itemView.GetComponent<Button>().onClick.AddListener(() => onClick.Invoke(itemView));
            }
        }

        private void Update()
        {
            ItemView newHover = ResolveGamepadHover();

            if (lastHover != newHover)
            {
                if (lastHover != null)
                    lastHover.UnHover();
                lastHover = newHover;
                if (lastHover != null)
                    lastHover.Hover();
            }

            if (Gamepad.current != null && Gamepad.current.aButton.wasPressedThisFrame && lastHover != null)
            {
                print("aButton");
                onClick.Invoke(lastHover);
            }
        }

        ItemView ResolveGamepadHover()
        {
            if (Gamepad.current == null)
                return null;

            float maxProjection = -2;
            int maxProjectionIdx = -1;

            Vector2 inputVector = Gamepad.current.leftStick.ReadValue().normalized;

            if (inputVector == Vector2.zero)
                return null;

            for (int i = 0; i < buttonDirections.Length; i++)
            {
                float projection = Utils.Projection(inputVector, buttonDirections[i]);
                if (projection > maxProjection)
                {
                    maxProjection = projection;
                    maxProjectionIdx = i;
                }
            }

            if (maxProjectionIdx == -1)
                throw new Exception("Projection not found");

            return itemViews[maxProjectionIdx];
        }
    }
}