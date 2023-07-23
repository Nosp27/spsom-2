using GameControl.StateMachine;
using UnityEngine;

namespace AI.States
{
    public class Dodge : BaseShipAIState
    {
        [SerializeField] private float dodgeDuration;
    
        private Vector3 m_DodgeTarget;
        private float m_DodgeTimer;

        public override void Tick()
        {
            if (!ShipAIControls.IsMoving())
            {
                Vector3 moveDirection =
                    (Quaternion.AngleAxis(Random.Range(-30, 30), Vector3.up) * transform.forward).normalized;
                m_DodgeTarget = transform.position + moveDirection * Random.Range(15, 30);
                ShipAIControls.MoveAt(m_DodgeTarget);
            }
            m_DodgeTimer += Time.deltaTime;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            ShipAIControls.thisShip.MovementService.CancelMovement();
            m_DodgeTimer = 0;
        }

        public override void OnExit()
        {
        
        }

        public override bool Done()
        {
            return m_DodgeTimer >= dodgeDuration;
        }
    }
}
