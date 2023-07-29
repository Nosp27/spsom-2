using Bonsai;
using UnityEngine;

namespace BonsaiAI.Conditions.DamageModelConditional
{
    [BonsaiNode("Conditional/DamageModel/", "Question")]
    public class CompareHealthPercent : BaseDamageModelConditional
    {
        [Range(0, 100)] private int thresholdPercent;
        [SerializeField] private bool lower = true;
        
        public override bool Condition()
        {
            int healthPercent = (int)(100f * damageModel.Health / damageModel.MaxHealth);
            if (lower)
                return healthPercent < thresholdPercent;

            return healthPercent >= thresholdPercent;
        }
    }
}