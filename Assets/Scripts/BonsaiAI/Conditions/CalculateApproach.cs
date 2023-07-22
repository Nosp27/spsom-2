using Bonsai;
using Bonsai.Core;
using BonsaiAI.Tasks;
using UnityEngine;

namespace BonsaiAI.Conditions
{
    [BonsaiNode("Tasks/", "Question")]
    public class CalculateRaycastApproach : Task
    {
        [SerializeField] private BB_KEY targetKey = BB_KEY.MINING_TARGET;
        [SerializeField] private BB_KEY outputKey = BB_KEY.APPROACH_TARGET;
        [SerializeField] private float distanceFromSurface = 100f;

        private Transform _cachedTarget;
        private Collider _cachedCollider;
    
        public override Status Run()
        {
            Transform _target = Blackboard.Get<Transform>(targetKey.ToString());
            
            if (_target == null)
            {
                return Status.Failure;
            }
            
            Vector3 target = Target(_target);
            Blackboard.Set(outputKey.ToString(), target);
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
