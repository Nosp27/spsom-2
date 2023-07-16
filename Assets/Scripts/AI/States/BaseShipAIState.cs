using GameControl.StateMachine;
using UnityEngine;

namespace AI.States
{
    public abstract class BaseShipAIState : BaseState, IState
    {
        protected ITargetDetector TargetDetector;
        public ShipAIControls ShipAIControls { get; protected set; }

        public override void OnEnter()
        {
            if (!ShipAIControls)
                ShipAIControls = GetComponentInParent<ShipAIControls>();

            if (TargetDetector == null)
                TargetDetector = GetComponentInParent<ITargetDetector>();
        }
        
        public virtual bool Done() => true;
    }
}
