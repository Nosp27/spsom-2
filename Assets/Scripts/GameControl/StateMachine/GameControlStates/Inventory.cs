using UI.Inventory.GUIMediators;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GameControl.StateMachine.GameControlStates
{
    public class Inventory : MonoBehaviour, IState
    {
        [SerializeField] private MultiSelectGUIMediator inventoryGuiMediator;
        [SerializeField] private GameObject hudCanvas;
        private CameraController camC;

        public void Tick()
        {
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
                inventoryGuiMediator.Run();
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