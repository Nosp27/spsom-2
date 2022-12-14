using UnityEngine;

namespace GameControl.StateMachine.GameControlStates
{
    public class Inventory : MonoBehaviour, IState
    {
        [SerializeField] private GameObject inventoryCanvas;
        [SerializeField] private GameObject playerHudCanvas;
        
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
            GameController.Current.SwitchCursorControl(!targetMode);
            inventoryCanvas.SetActive(targetMode);
            playerHudCanvas.SetActive(!targetMode);
            Cursor.visible = targetMode;
            Time.timeScale = targetMode ? 0 : 1;
        }
    }
}
