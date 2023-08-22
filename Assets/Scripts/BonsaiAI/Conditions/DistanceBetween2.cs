using System.ComponentModel;
using System.Text;
using Bonsai;
using Bonsai.Core;
using UnityEngine;

namespace BonsaiAI.Conditions
{
    [BonsaiNode("Conditional/Transform/", "Question")]
    public class DistanceBetween2 : ConditionalTask
    {
        [SerializeField] private float threshold = 100;
        [SerializeField] protected BBKey from;
        [SerializeField] protected BBKey to;

        [SerializeField] private bool successWhenCloser = true;

        [ReadOnly(true)][SerializeField]private float lastDistance;
        
        public override bool Condition()
        {
            Vector3 fromPosition = Utils.Position(Blackboard.Get(from));
            Vector3 toPosition = Utils.Position(Blackboard.Get(to));

            if (fromPosition == default || toPosition == default)
                return false;

            lastDistance = Vector3.Distance(fromPosition, toPosition);
            return successWhenCloser == (lastDistance < threshold);
        }
    }
}