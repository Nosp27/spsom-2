using System.Collections;
using UnityEngine;

namespace AI
{
    public enum DetectionMethod
    {
        ONLY_PLAYER,
        EXCEPT_PLAYER,
        EVERYONE,
        NOBODY,
    }

    public class EnemyDetector : MonoBehaviour
    {
        [SerializeField] private DetectionMethod detectionMethod;
        [SerializeField] private float discoveryRange = 300;
        [SerializeField] private float loseRange = 500;

        public Ship Enemy { get; private set; }
        private Ship ThisShip;

        private IEnumerator Start()
        {
            ThisShip = GetComponentInParent<Ship>();

            yield return SeekEnemyShip();
        }

        IEnumerator SeekEnemyShip()
        {
            while (true)
            {
                yield return new WaitForSeconds(.2f);
                if (Enemy && (Enemy.transform.position - transform.position).magnitude > loseRange)
                    Enemy = null;
                
                if (Enemy && Enemy.Alive)
                    continue;

                Collider[] colliders = Physics.OverlapSphere(transform.position, discoveryRange);
                Ship nearestShip = null;
                float nearestShipDistance = discoveryRange;
                foreach (var col in colliders)
                {
                    Ship ship = col.GetComponentInParent<Ship>();
                    if (IsEnemy(ship))
                    {
                        float distance = (ship.transform.position - transform.position).magnitude;
                        if (nearestShip == null || distance < nearestShipDistance)
                        {
                            nearestShip = ship;
                            nearestShipDistance = distance;
                        }
                    }
                    Enemy = nearestShip;
                }
            }
        }

        public bool IsEnemy(Ship ship)
        {
            if (detectionMethod == DetectionMethod.NOBODY)
                return false;

            if (!ship || !ship.Alive || ship == ThisShip)
                return false;

            if (detectionMethod == DetectionMethod.EVERYONE)
                return true;

            if (detectionMethod == DetectionMethod.ONLY_PLAYER)
                return ship.isPlayerShip;

            if (detectionMethod == DetectionMethod.EXCEPT_PLAYER)
                return !ship.isPlayerShip;

            return false;
        }
    }
}