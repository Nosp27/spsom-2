using UI.Inventory;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameControl.StateMachine.GameControlStates
{
    public class Inventory : MonoBehaviour, IState
    {
        [SerializeField] private SlotGuiMediator inventoryGuiMediator;
        [SerializeField] private GameObject hudCanvas;
        private CameraController camC;

        public void Tick()
        {
            if (Gamepad.current != null)
            {
                int delta = 0;
                if (Gamepad.current.rightShoulder.isPressed)
                    delta = 1;
                if (Gamepad.current.leftShoulder.isPressed)
                    delta = -1;
                if (delta != 0)
                    inventoryGuiMediator.SwitchTab(delta);
            }
        }

        public void OnEnter()
        {
            SwitchInventoryMode(true);
        }

        public void OnExit()
        {
            SwitchInventoryMode(false);
        }

        void SwitchInventoryMode(bool targetMode)
        {
            hudCanvas.SetActive(!targetMode);
            camC = FindObjectOfType<CameraController>();
            var zoomController = camC.GetComponent<CameraZoomControl>();
            if (zoomController)
                zoomController.enabled = !targetMode;
            if (targetMode)
            {
                camC.Zoom = 0.45f;
                inventoryGuiMediator.Run();
                if (Gamepad.current == null)
                    inventoryGuiMediator.UnfreezeAllTabs();
                else
                    inventoryGuiMediator.SwitchTab(0);
            }
            else
            {
                inventoryGuiMediator.Stop();
            }

            GameController.Current.SwitchCursorControl(!targetMode);
            Cursor.visible = targetMode;
            Time.timeScale = targetMode ? 0 : 1;
        }

        public bool Done() => true;
    }
}