using System.Collections.Generic;
using UnityEngine;

namespace GameControl.StateProcessors
{
    public class FacilitySP : StateProcessor
    {
        public GizmoController gizmoController;

        public InputState currentState;

        public override List<InputState> RelevantInputStates()
        {
            return new List<InputState>() {InputState.START_FACILITY, InputState.FACILITY, InputState.LEAVE_FACILITY};
        }

        public override InputState GetInputState(InputState state)
        {
            if (currentState == InputState.LEAVE_FACILITY)
            {
                currentState = InputState.NOPE;
                return InputState.NOPE;
            }
            
            if (
                state == InputState.START_FACILITY || state == InputState.FACILITY ||
                gizmoController.selectedFacility != null
            )
            {
                if (Input.GetKeyDown(KeyCode.E))
                    currentState = InputState.START_FACILITY;
                else
                    currentState = InputState.NOPE;
            }
            else
            {
                currentState = InputState.NOPE;
            }

            return currentState;
        }

        public override void ProcessState(InputState state)
        {
            switch (state)
            {
                // case InputState.START_FACILITY:
                //     ShopUI shopUI = Instantiate(gizmoController.selectedFacility.CanvasPrefab).GetComponent<ShopUI>();
                //
                //     Cursor.visible = true;
                //     shopUI.gameObject.SetActive(true);
                //     shopUI.ConfirmCallback = () =>
                //     {
                //         Destroy(shopUI.gameObject);
                //         Cursor.visible = false;
                //         currentState = InputState.LEAVE_FACILITY;
                //     };
                //     shopUI.GetComponent<Animator>().Play("MenuStart");
                //     break;
            }
        }
    }
}