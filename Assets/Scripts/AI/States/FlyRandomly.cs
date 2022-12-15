using UnityEngine;

namespace AI.States
{
    public class FlyRandomly : BaseShipAIState
    {
        [SerializeField] private float minMoveRange;
        [SerializeField] private float maxMoveRange;
        [SerializeField] private float angleDeviation;
    
        private Ship m_ThisShip;
        private Vector3 m_AnywhereMovePoint;

        public override void Tick()
        {
            if (!m_ThisShip.IsMoving())
                m_AnywhereMovePoint = Vector3.zero;
        
            if (m_AnywhereMovePoint == Vector3.zero)
            {
                m_AnywhereMovePoint = CreateMoveTarget();
            }
            ShipAIControls.MoveAt(m_AnywhereMovePoint);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            m_AnywhereMovePoint = Vector3.zero;
            m_ThisShip = GetComponentInParent<Ship>();
        }

        public override void OnExit()
        {
        
        }

        private Vector3 CreateMoveTarget()
        {
            Vector3 moveDirection =
                (Quaternion.AngleAxis(Random.Range(-angleDeviation / 2f, angleDeviation / 2f), Vector3.up) *
                 transform.forward).normalized;
            return transform.position + moveDirection * Random.Range(minMoveRange, maxMoveRange);
        }
    }
}
