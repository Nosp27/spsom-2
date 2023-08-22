using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace BonsaiAI.Conditions
{
    [BonsaiNode("Conditional/Transform/", "Question")]
    public class Distance : ConditionalTask
    {
        [SerializeField] private float threshold = 100;
        [SerializeField] protected BBKey key;
        private Vector3 targetPosition => Utils.Position(Blackboard.Get(key));

        [SerializeField] private bool successWhenCloser = true;

        public override bool Condition()
        {
            if (targetPosition == default)
                return false;

            float distance = Vector3.Distance(targetPosition, Actor.transform.position);

            if (successWhenCloser)
                return distance < threshold;

            return distance >= threshold;
        }
    }
}