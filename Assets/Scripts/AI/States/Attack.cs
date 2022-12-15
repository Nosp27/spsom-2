using UnityEngine;

namespace AI.States
{
    public class Attack : BaseShipAIState
    {
        public override void Tick()
        {
            Ship thisShip = ShipAIControls.thisShip;
            Vector3 aimTarget = EnemyDetector.Enemy.transform.position;
            thisShip.Aim(aimTarget);
            thisShip.TurnOnPlace(aimTarget);
            if (thisShip.Aimed(aimTarget))
                thisShip.Shoot(aimTarget);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ShipAIControls.thisShip.CancelMovement();
        }

        public override void OnExit()
        {
        }
    }
}