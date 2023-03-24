using System;
using UI.Inventory.ItemViews;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace UI.Inventory.ItemViewOrganizers
{
    public class CircularSlotCanvasOrganizer : AbstractSlotCanvasOrganizer
    {
        private bool isFrozen;
        private bool arrangeDone;

        private SelectableCanvasItemView[] buttons;
        private Vector2[] buttonDirections;

        public int maxHighlighted = 2;
        private int currentHighlighted;


        // Only for visuals - not to select same button every frame
        private SelectableCanvasItemView selected;

        public override void Arrange()
        {
            Vector3 center = transform.position;
            buttons = GetComponentsInChildren<SelectableCanvasItemView>();
            buttonDirections = new Vector2[buttons.Length];

            int i = 0;
            foreach (ItemView button in buttons)
            { 
                button.UnHighlight();
                buttonDirections[i] = ((Vector2) (button.transform.position - center)).normalized;
                i++;
            }

            currentHighlighted = 0;
            arrangeDone = true;
        }

        private void OnDisable()
        {
            arrangeDone = false;
            if (buttons != null)
            {
                foreach (SelectableCanvasItemView button in buttons)
                {
                    button.UnHighlight();
                }
            }
        }

        private void Update()
        {
            if (!arrangeDone)
                return;
            SelectableCanvasItemView newSelected = ResolveGamepadHover();
            
            if (newSelected != null && selected != newSelected)
            {
                selected = newSelected;
                newSelected.GetComponent<Button>().Select();
            }
            if (Gamepad.current != null && Gamepad.current.xButton.wasPressedThisFrame && selected != null)
            {
                print("OCC");
                if (selected.occupied)
                {
                    selected.UnHighlight();
                    currentHighlighted--;
                }
                else
                {
                    if (currentHighlighted < maxHighlighted && selected.GetItem() != null)
                    {
                        selected.Highlight();
                        currentHighlighted++;
                    }
                }
            }
        }

        SelectableCanvasItemView ResolveGamepadHover()
        {
            Vector2 offset = (Vector2) transform.position + new Vector2(30, 20);
            if (Gamepad.current == null)
                return null;

            float maxProjection = -2;
            int maxProjectionIdx = -1;

            Vector2 inputVector = Gamepad.current.leftStick.ReadValue().normalized;

            if (inputVector == Vector2.zero)
                return null;

            Debug.DrawRay(offset, inputVector * 100, Color.blue);

            for (int i = 0; i < buttonDirections.Length; i++)
            {
                float projection = Utils.Projection(inputVector, buttonDirections[i]);
                Debug.DrawRay(offset, buttonDirections[i] * 100, Color.yellow);
                if (projection > maxProjection)
                {
                    maxProjection = projection;
                    maxProjectionIdx = i;
                }
            }

            if (maxProjectionIdx == -1)
                throw new Exception("Projection not found");

            if (selected == buttons[maxProjectionIdx])
                return null;

            if (maxProjectionIdx == 7)
                print($"7 selected : {buttons[maxProjectionIdx].gameObject.name}");
            return buttons[maxProjectionIdx];
        }

        public override void Freeze()
        {
            isFrozen = true;
            foreach (var itemView in GetComponentsInChildren<ItemView>())
            {
                itemView.GetComponent<Button>().interactable = false;
            }
        }

        public override void Unfreeze()
        {
            isFrozen = false;
            foreach (var itemView in GetComponentsInChildren<ItemView>())
            {
                itemView.GetComponent<Button>().interactable = true;
            }
        }
    }
}