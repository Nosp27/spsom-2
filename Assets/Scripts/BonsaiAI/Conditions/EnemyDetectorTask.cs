using System;
using AI;
using Bonsai;
using BonsaiAI.Tasks;
using UnityEngine;

namespace BonsaiAI.Conditions
{
    [BonsaiNode("Tasks/", "Question")]
    public class EnemyDetectorTask : Bonsai.Core.ConditionalTask
    {
        private EnemyDetector detector;
        [SerializeField] private BBKey enemyTransformOutputKey;
        [SerializeField] private BBKey enemyDamageModelOutputKey;

        public override void OnStart()
        {
            detector = Actor.GetComponent<EnemyDetector>();
        }

        public override bool Condition()
        {
            if (detector == null)
            {
                throw new Exception($"No detector for {Actor.name}");
            }

            var target = detector.Target;

            if (target == null)
            {
                return false;
            }

            Blackboard.Set(enemyTransformOutputKey, target.transform);
            Blackboard.Set(enemyDamageModelOutputKey, target);
            return true;
        }
    }
}