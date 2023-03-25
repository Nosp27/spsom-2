using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(CollisionAvoidance))]
    public class ShipAIControls : MonoBehaviour
    {
        public Ship thisShip { get; private set; }
        private CollisionAvoidance m_CA;
        private Vector3 m_AvoidPoint;

        private void Start()
        {
            thisShip = GetComponent<Ship>();
            m_CA = GetComponent<CollisionAvoidance>();
        }

        public void MoveAt(Vector3 point, float throttleCutoff=1)
        {
            if (point != Vector3.zero)
            {
                m_AvoidPoint = m_CA.AvoidPointCompound(point);
                Debug.DrawLine(transform.position, m_AvoidPoint, Color.yellow);
            }
        
            if (!thisShip.IsMoving() && m_AvoidPoint != Vector3.zero) {
                m_AvoidPoint = Vector3.zero;
            }

            if (m_AvoidPoint == Vector3.zero)
            {
                thisShip.Move(point, throttleCutoff);
            }
            else
            {
                thisShip.Move(m_AvoidPoint, throttleCutoff);
            }
        }

        public bool IsMoving()
        {
            return thisShip.IsMoving();
        }
    }
}
