using UnityEngine;

namespace GameControl.StateMachine.GameControlStates
{
    public class Inventory : MonoBehaviour, IState
    {
        [SerializeField] private GameObject inventoryCanvas;
        [SerializeField] private GameObject playerHudCanvas;
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
            camC = FindObjectOfType<CameraController>();
            var zoomController = camC.GetComponent<CameraZoomControl>();
            if (zoomController)
                zoomController.enabled = !targetMode;
            if (targetMode)
                camC.Zoom = 0.4f;

            GameController.Current.SwitchCursorControl(!targetMode);
            inventoryCanvas.SetActive(targetMode);
            playerHudCanvas.SetActive(!targetMode);
            Cursor.visible = targetMode;
            Time.timeScale = targetMode ? 0 : 1;
        }

        public bool Done() => true;
    }
}