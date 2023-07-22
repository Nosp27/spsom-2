using System.Collections.Generic;
using System.Linq;
using Bonsai;
using UnityEngine;

namespace BonsaiAI.Tasks.Mining
{
    [BonsaiNode("Tasks/", "Question")]
    public class MiningZoneDetector : AiShipTask
    {
        [SerializeField] private BB_KEY outputKey = BB_KEY.MINING_TARGET;
        [SerializeField] private LayerMask zoneLayerMask;
        [SerializeField] private LayerMask shipsLayerMask;
        [SerializeField] private float detectionRange = 500;
        [SerializeField] private float occupationRange = 150;

        private Collider[] buffer = new Collider[10];
    
        private Transform TheLeastOccupied()
        {
            IEnumerable<Transform> allZones = DetectZones();
            return allZones.OrderBy(x => ZoneScore(x)).FirstOrDefault();
        }

        private float ZoneScore(Transform zone)
        {
            return Physics.OverlapSphereNonAlloc(zone.position, occupationRange, buffer, shipsLayerMask);
        }
    
        private IEnumerable<Transform> DetectZones()
        {
            return Physics
                .OverlapSphere(Actor.transform.position, detectionRange, zoneLayerMask)
                .Select(x => x.transform);
        }

        public override Status Run()
        {
            Transform zone = TheLeastOccupied();
            if (zone == null)
                return Status.Failure;
            BlackboardSet(outputKey, zone);
            return Status.Success;
        }
    }
}