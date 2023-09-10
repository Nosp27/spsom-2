using UnityEngine;

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
            sm.AddTransition(movementState, movementState, () => false);
        }
    }
}
