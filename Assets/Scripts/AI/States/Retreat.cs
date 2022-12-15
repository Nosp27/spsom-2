using UnityEngine;

namespace AI.States
{
    public class Retreat : BaseShipAIState
    {
        private Vector3 m_DodgeVector;
        private Ship enemy => EnemyDetector?.Enemy;

        public override void Tick()
        {
            if (m_DodgeVector == Vector3.zero || !ShipAIControls.IsMoving())
            {
                m_DodgeVector = GetDodgeVector();
                ShipAIControls.MoveAt(m_DodgeVector);
            }
        }

        public override void OnExit()
        {
            m_DodgeVector = Vector3.zero;
        }

        Vector3 GetDodgeVector()
        {
            if (m_DodgeVector != Vector3.zero && (m_DodgeVector - transform.position).magnitude > 20)
                return m_DodgeVector;
            float dodgeDirectionSign = Random.value < 0.5f ? 1 : -1;

            Vector3 attackVector = transform.position - enemy.transform.position;
            Vector3 normal = Vector3.Cross(attackVector, Vector3.up) * dodgeDirectionSign;
            m_DodgeVector = transform.position + normal + attackVector;
            return m_DodgeVector;
        }
    }
}