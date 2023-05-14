using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpaceItems.ProceduralStations
{
    public enum StationModuleTag
    {
        LARGE_MODULE,
        MEDUIM_MODULE,
        SMALL_MODULE,
        BRIDGE,
    }
    
    public class StationModule : MonoBehaviour
    {
        private Tuple<Vector3, Vector3> boundsForOverlap;
        [SerializeField] private StationModuleTag[] _tags;
        [SerializeField] private Transform engagePointsParent;
        
        public StationModuleTag[] tags => _tags;
        public EngagePoint[] engagePoints => engagePointsParent.GetComponentsInChildren<EngagePoint>();

        private HashSet<StationModule> m_EngagedModules = new HashSet<StationModule>();

        private HashSet<Collider> ignoredColliders = new HashSet<Collider>();
        private Collider[] colliders;

        public void Init()
        {
            colliders = GetComponentsInChildren<Collider>();
            ignoredColliders.UnionWith(colliders);
            foreach (EngagePoint engagePoint in engagePoints)
            {
                engagePoint.SetAttachedModule(this);
            }
        }

        public void Engage(StationModule module)
        {
            m_EngagedModules.Add(module);
            ignoredColliders.UnionWith(module.colliders);
        }

        public bool Collides(float spacing)
        {
            foreach (Collider col in colliders)
            {
                Collider[] overlappedCols = Physics.OverlapBox(col.bounds.center, col.bounds.extents + Vector3.one * spacing, col.transform.rotation);
                foreach (Collider overlappedCol in overlappedCols)
                {
                    if (!ignoredColliders.Contains(overlappedCol) &&
                        overlappedCol.GetComponentInParent<StationModule>())
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void OnDrawGizmos()
        {
            if (engagePoints == null)
                return;
            
            Color yellow = Color.yellow;
            yellow.a = 0.6f;
            Color green = Color.green;
            green.a = 0.6f;

            Gizmos.color = yellow;
            foreach (EngagePoint point in engagePoints)
            {
                Gizmos.DrawSphere(point.transform.position, 1.2f);
            }
            Gizmos.color = green;
            Gizmos.DrawSphere(transform.position, 0.4f);
        }
    }
}
