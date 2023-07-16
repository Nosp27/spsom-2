using UnityEngine;

namespace AI.States
{
    public class Attack : BaseShipAIState
    {
        public override void Tick()
        {
            Ship thisShip = ShipAIControls.thisShip;
            Transform aimTarget = TargetDetector.Target.transform;
            thisShip.TurnOnPlace(aimTarget.position);
            if (thisShip.Aimed())
                thisShip.Fire(aimTarget.position);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            Ship thisShip = ShipAIControls.thisShip;
            Transform aimTarget = TargetDetector.Target.transform;
            thisShip.Track(aimTarget);
            thisShip.CancelMovement();
        }

        public override void OnExit()
        {
            ShipAIControls?.thisShip?.Track(null);
        }
    }
}