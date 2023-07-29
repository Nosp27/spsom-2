using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace BonsaiAI.Conditions
{
    [BonsaiNode("Tasks/", "Question")]
    public class CalculateRaycastApproach : Task
    {
        [SerializeField] private BBKey targetKey;
        [SerializeField] private BBKey outputKey;
        [SerializeField] private float distanceFromSurface = 100f;

        private Transform _cachedTarget;
        private Collider _cachedCollider;
    
        public override Status Run()
        {
            Transform _target = Blackboard.Get<Transform>(targetKey);
            
            if (_target == null)
            {
                return Status.Failure;
            }
            
            Vector3 target = Target(_target);
            Blackboard.Set(outputKey, target);
            return Status.Success;
        }

        Vector3 Target(Transform target)
        {
            if (target != _cachedTarget)
            {
                _cachedTarget = target;
                _cachedCollider = target.GetComponent<Collider>();
            }

            Vector3 actorPosition = Actor.transform.position;
            Ray ray = new Ray(actorPosition, target.position - actorPosition);
            RaycastHit hit;

            if (!_cachedCollider.Raycast(ray, out hit, distanceFromSurface * 3))
                return target.position;

            var result = hit.point + (actorPosition - hit.point).normalized * distanceFromSurface;
            return result;
        }
        
        
    }
}
