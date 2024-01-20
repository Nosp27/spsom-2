using System;
using GameControl.InputHandlers;
using GameEventSystem;
using UnityEngine;

namespace GameControl
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerShipControl : MonoBehaviour
    {
        private PlayerInput playerInput;
        [SerializeField] private Ship shipUnderControl;

        private bool canControl;

        private void Start()
        {
            playerInput = GetComponent<PlayerInput>();

            EventLibrary.shipSpawned.AddListener(s =>
            {
                if (s == shipUnderControl) canControl = true;
                s.MovementService.LimitThrottle(1);
            });

            EventLibrary.inputMoveActionPerformed.AddListener(inp =>
            {
                if (inp != playerInput || !canControl) return;
                shipUnderControl.MovementService.LimitThrottle(1);
                shipUnderControl.MovementService.Move(playerInput.CursorPosition);
            });

            EventLibrary.inputShootActionPerformed.AddListener(inp =>
            {
                if (inp != playerInput || !canControl) return;
                shipUnderControl.Fire(playerInput.CursorPosition);
            });

            EventLibrary.inputStopMovementActionPerformed.AddListener(inp =>
            {
                if (inp != playerInput || !canControl) return;
                shipUnderControl.MovementService.CancelMovement(false);
            });

            EventLibrary.inputBrakeActionPerformed.AddListener(inp =>
            {
                if (inp != playerInput || !canControl) return;
                shipUnderControl.MovementService.LimitThrottle(1);
                shipUnderControl.MovementService.CancelMovement();
            });
        }

        private void Update()
        {
            if (canControl)
            {
                shipUnderControl.MovementService.TurnAt(playerInput.CursorPosition);
                shipUnderControl.Aim(playerInput.CursorPosition);
            }
        }
    }
}