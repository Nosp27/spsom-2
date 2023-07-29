using Bonsai;
using Bonsai.Core;
using BonsaiAI.Tasks;
using UnityEngine;

namespace BonsaiAI.Conditions
{
    [BonsaiNode("Conditional/Transform/", "Question")]
    public class Distance : ConditionalTask
    {
        [SerializeField] private float threshold = 100;
        [SerializeField] protected BB_KEY key = BB_KEY.MOVE_TARGET;
        private Transform target => Blackboard.Get<Transform>(key.ToString());

        private bool successWhenCloser = true;

        public override bool Condition()
        {
            if (target == null)
                return false;

            float distance = Vector3.Distance(target.position, Actor.transform.position);

            if (successWhenCloser)
                return distance < threshold;

            return distance >= threshold;
        }
    }
}