using UnityEngine;

namespace AI.States
{
    public class Chase : BaseShipAIState
    {
        public override void Tick()
        {
            Ship enemy = EnemyDetector.Enemy;
            if (!enemy)
                return;
            
            Vector3 target = enemy.transform.position;
            ShipAIControls.MoveAt(target);
        }

        public override void OnExit()
        {
            
        }
    }
}
