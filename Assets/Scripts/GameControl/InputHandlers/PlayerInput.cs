using GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameControl.InputHandlers
{
    public class PlayerInput : MonoBehaviour
    {
        private Collider zeroPlane;
        private Camera Cam => Camera.main;
        
        public Vector3 CursorPosition { get; private set; }

        private void Start()
        {
            zeroPlane = GameObject.Find("ZeroPlane").GetComponent<Collider>();
        }

        private void Update()
        {
            CursorPosition = GetCursorPosition();
            if (Keyboard.current.wKey.isPressed)
            {
                EventLibrary.inputMoveActionPerformed.Invoke(this);
            }
            else if (Keyboard.current.wKey.wasReleasedThisFrame)
            {
                EventLibrary.inputStopMovementActionPerformed.Invoke(this);
            }
            else if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                EventLibrary.inputBrakeActionPerformed.Invoke(this);
            }
        }

        private Vector3 GetCursorPosition()
        {
            Ray ray = Cam.ScreenPointToRay(Mouse.current.position.value);
            RaycastHit hit;

            if (zeroPlane.Raycast(ray, out hit, 1000) && hit.collider != null)
            {
                return hit.point;
            }

            return default;
        }
    }
}