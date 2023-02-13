using GameControl.StateMachine;
using UnityEngine;

namespace AI.States
{
    public abstract class BaseShipAIState : BaseState, IState
    {
        protected IEnemyDetector EnemyDetector;
        public ShipAIControls ShipAIControls { get; protected set; }

        public override void OnEnter()
        {
            if (!ShipAIControls)
                ShipAIControls = GetComponentInParent<ShipAIControls>();

            if (EnemyDetector == null)
                EnemyDetector = GetComponentInParent<IEnemyDetector>();
        }
        
        public virtual bool Done() => true;
    }
}
