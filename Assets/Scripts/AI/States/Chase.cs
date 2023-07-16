using UnityEngine;

namespace AI.States
{
    public class Chase : BaseShipAIState
    {
        [SerializeField] private float throttleCutoff=1;
        public override void Tick()
        {
            var enemy = TargetDetector.Target;
            if (!enemy)
                return;
            
            Vector3 target = enemy.transform.position;
            ShipAIControls.MoveAt(target, throttleCutoff);
        }

        public override void OnExit()
        {
            
        }
    }
}
