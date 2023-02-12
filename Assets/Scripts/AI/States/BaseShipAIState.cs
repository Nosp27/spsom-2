using GameControl.StateMachine;
using UnityEngine;

namespace AI.States
{
    public abstract class BaseShipAIState : BaseState, IState
    {
        protected EnemyDetector EnemyDetector;
        public ShipAIControls ShipAIControls { get; protected set; }

        public override void OnEnter()
        {
            if (!ShipAIControls)
                ShipAIControls = GetComponentInParent<ShipAIControls>();

            if (!EnemyDetector)
                EnemyDetector = GetComponentInParent<EnemyDetector>();
        }
        
        public virtual bool Done() => true;
    }
}
