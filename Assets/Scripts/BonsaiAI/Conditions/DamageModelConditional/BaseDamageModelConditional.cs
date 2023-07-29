using Bonsai.Core;
using BonsaiAI.Tasks;

namespace BonsaiAI.Conditions.DamageModelConditional
{
    public abstract class BaseDamageModelConditional : ConditionalTask
    {
        private BB_KEY key;
        protected DamageModel damageModel;

        public override void OnStart()
        {
            damageModel = Blackboard.Get<DamageModel>(key.ToString());   
            base.OnStart();
        }
    }
}
