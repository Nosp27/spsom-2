using Bonsai;

namespace BonsaiAI.Conditions.DamageModelConditional
{
    [BonsaiNode("Conditional/DamageModel/", "Question")]
    public class IsAlive : BaseDamageModelConditional
    {
        public override bool Condition()
        {
            return damageModel.Alive;
        }
    }
}
