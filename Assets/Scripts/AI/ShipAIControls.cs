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
            if (GameController.Current)
                GameController.Current.OnShipChange.AddListener(OnShipChange);
        }

        void OnShipChange(Ship old, Ship _new)
        {
            isAiEnabled = _new == thisShip;
        }

        public void MoveAt(Vector3 point)
        {
            if (point != Vector3.zero)
            {
                m_AvoidPoint = m_CA.AvoidPointCompound(point);
            }
        
            if (!thisShip.MovementService.IsMoving() && m_AvoidPoint != Vector3.zero) {
                m_AvoidPoint = Vector3.zero;
            }

            if (m_AvoidPoint == Vector3.zero)
            {
                thisShip.MovementService.Move(point);
            }
            else
            {
                thisShip.MovementService.Move(m_AvoidPoint);
            }
        }

        public bool IsMoving()
        {
            return thisShip.MovementService.IsMoving();
        }
    }
}
