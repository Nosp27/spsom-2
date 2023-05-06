using UI.Inventory;
using UnityEngine;

namespace GameControl.StateMachine.GameControlStates
{
    public class Inventory : MonoBehaviour, IState
    {
        [SerializeField] private InventoryGui gui;
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
            if (targetMode)
            {
                gui.Run();
            }
            else
            {
                gui.Stop();
            }

            GameController.Current.SwitchCursorControl(!targetMode);
            Cursor.visible = targetMode;
            Time.timeScale = targetMode ? 0 : 1;
        }

        public bool Done() => true;
    }
}