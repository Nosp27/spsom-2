using System;
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

    public class EnemyDetector : MonoBehaviour, IEnemyDetector
    {
        [SerializeField] private DetectionMethod detectionMethod;
        [SerializeField] private float discoveryRange = 300;
        [SerializeField] private float loseRange = 500;

        [SerializeField] private bool seekNearest = true;

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
                
                // Loose enemy from sight if it is far away
                if (Enemy && (Enemy.transform.position - transform.position).magnitude > loseRange)
                    Enemy = null;

                // Do not seek enemy if it is already found
                if (Enemy && Enemy.Alive)
                    continue;

                // Seek all colliders within discovery range and find nearest belonging to a ship
                Collider[] colliders = Physics.OverlapSphere(transform.position, discoveryRange);
                Ship nearestShip = null;
                float nearestShipDistance = seekNearest ? discoveryRange : 0;
                float comparisonMultiplier = seekNearest ? 1 : -1;

                foreach (var col in colliders)
                {
                    Ship ship = col.GetComponentInParent<Ship>();
                    if (IsEnemy(ship))
                    {
                        float distance = (ship.transform.position - transform.position).magnitude;
                        if (
                            nearestShip == null
                            || (comparisonMultiplier * distance < comparisonMultiplier * nearestShipDistance)
                        )
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
            if (ship == ThisShip)
                return false;

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