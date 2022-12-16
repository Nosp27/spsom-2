using UnityEngine;

namespace AI.States
{
    public class Attack : BaseShipAIState
    {
        public override void Tick()
        {
            Ship thisShip = ShipAIControls.thisShip;
            Transform aimTarget = EnemyDetector.Enemy.transform;
            thisShip.Track(aimTarget);
            thisShip.TurnOnPlace(aimTarget.position);
            if (thisShip.Aimed())
                thisShip.Fire(aimTarget.position);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ShipAIControls.thisShip.CancelMovement();
        }

        public override void OnExit()
        {
            ShipAIControls?.thisShip?.Track(null);
        }
    }
}