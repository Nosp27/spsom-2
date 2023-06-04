using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(CollisionAvoidance))]
    public class ShipAIControls : MonoBehaviour
    {
        public Ship thisShip { get; private set; }
        private CollisionAvoidance m_CA;
        private Vector3 m_AvoidPoint;
        public bool isAiEnabled { get; private set; }

        private void Start()
        {
            isAiEnabled = true;
            thisShip = GetComponent<Ship>();
            m_CA = GetComponent<CollisionAvoidance>();
            GameController.Current.OnShipChange.AddListener(OnShipChange);
        }

        void OnShipChange(Ship old, Ship _new)
        {
            isAiEnabled = _new == thisShip;
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
