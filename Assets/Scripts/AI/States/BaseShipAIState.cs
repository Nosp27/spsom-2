using GameControl.StateMachine;
using UnityEngine;

namespace AI.States
{
    public abstract class BaseShipAIState : MonoBehaviour, IState
    {
        protected EnemyDetector EnemyDetector;
        protected ShipAIControls ShipAIControls;

        public abstract void Tick();

        public virtual void OnEnter()
        {
            if (!ShipAIControls)
                ShipAIControls = GetComponentInParent<ShipAIControls>();

            if (!EnemyDetector)
                EnemyDetector = GetComponentInParent<EnemyDetector>();
        }

        public abstract void OnExit();
        public virtual bool Done() => true;
    }
}
