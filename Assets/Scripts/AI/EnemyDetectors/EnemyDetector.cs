using System;
using System.Collections;
using Factions;
using UnityEngine;

namespace AI
{
    public enum DetectionMethod
    {
        ONLY_FACTION,
        EXCEPT_FACTION,
        EVERYONE,
        NOBODY,
    }

    public class EnemyDetector : MonoBehaviour, ITargetDetector
    {
        [SerializeField] private SpsomFaction referenceFaction;
        [SerializeField] private DetectionMethod detectionMethod;
        [SerializeField] private float discoveryRange = 300;
        [SerializeField] private float loseRange = 500;

        [SerializeField] private bool seekNearest = true;

        public DamageModel Target { get; private set; }
        private DamageModel m_ThisDamageModel;

        private IEnumerator Start()
        {
            m_ThisDamageModel = GetComponentInParent<DamageModel>();

            yield return SeekEnemyShip();
        }

        IEnumerator SeekEnemyShip()
        {
            while (true)
            {
                yield return new WaitForSeconds(.2f);
                
                // Loose enemy from sight if it is far away
                if (Target && (Target.transform.position - transform.position).magnitude > loseRange)
                    Target = null;

                // Do not seek enemy if it is already found
                if (Target && Target.Alive)
                    continue;

                // Seek all colliders within discovery range and find nearest belonging to a ship
                Collider[] colliders = Physics.OverlapSphere(transform.position, discoveryRange);
                DamageModel nearestShip = null;
                float nearestShipDistance = seekNearest ? discoveryRange : 0;
                float comparisonMultiplier = seekNearest ? 1 : -1;

                foreach (var col in colliders)
                {
                    DamageModel ship = col.GetComponentInParent<DamageModel>();
                    SpsomFaction? shipFaction = ship != null ? 
                        ship.GetComponent<FactionMember>()?.Faction
                        : (SpsomFaction?) null;
                    if (shipFaction.HasValue && IsEnemy(ship, shipFaction.Value))
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

                    Target = nearestShip;
                }
            }
        }

        public bool IsEnemy(DamageModel shipDamageModel, SpsomFaction faction)
        {
            if (shipDamageModel == m_ThisDamageModel)
                return false;

            if (detectionMethod == DetectionMethod.NOBODY)
                return false;

            if (!shipDamageModel || !shipDamageModel.Alive)
                return false;

            if (detectionMethod == DetectionMethod.EVERYONE)
                return true;

            if (detectionMethod == DetectionMethod.ONLY_FACTION)
                return faction == referenceFaction;

            if (detectionMethod == DetectionMethod.EXCEPT_FACTION)
                return faction != referenceFaction;

            return false;
        }
    }
}