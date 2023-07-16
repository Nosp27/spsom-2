using UnityEngine;

namespace AI.States
{
    public class FlyAway : BaseShipAIState
    {
        public override void Tick()
        {
            var enemy = TargetDetector.Target;
            if (enemy == null)
                return;
            Vector3 lookAway = (transform.position - enemy.transform.position).normalized;
            ShipAIControls.MoveAt(transform.position + lookAway * 50);
        }

        public override void OnExit()
        {
        
        }
    }
}
