using Bonsai.Core;
using BonsaiAI.Tasks;
using UnityEngine;

namespace BonsaiAI.Conditions.DamageModelConditional
{
    public abstract class BaseDamageModelConditional : ConditionalTask
    {
        [SerializeField] private BBKey key;
        protected DamageModel damageModel;

        public override void OnStart()
        {
            damageModel = Blackboard.Get<DamageModel>(key);   
            base.OnStart();
        }
    }
}
