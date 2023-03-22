using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameControl.StateMachine.GameControlStates
{
    public class GameControlStateMachine : MonoBehaviour
    {
        [SerializeField] private Movement movementState;
        [SerializeField] private Inventory inventoryState;
        
        private StateMachine sm;

        void Start()
        {
            InitStateMachine();
        }

        private void Update()
        {
            sm.Tick();
        }

        void InitStateMachine()
        {
            sm = new StateMachine();

            Func<bool> InventoryTransitionPredicate = () =>
                Keyboard.current.eKey.wasPressedThisFrame || (Gamepad.current != null && Gamepad.current.triangleButton.wasPressedThisFrame);

            sm.AddTransition(movementState, inventoryState, InventoryTransitionPredicate);
            sm.AddTransition(inventoryState, movementState, InventoryTransitionPredicate);
        }
    }
}
