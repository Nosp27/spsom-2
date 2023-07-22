using AI;
using Bonsai.Core;

namespace BonsaiAI.Tasks
{
    public enum BB_KEY
    {
        MOVE_TARGET,
        ATTACK_TARGET,
        MINING_TARGET,
        APPROACH_TARGET,
    }

    public abstract class AiShipTask : Task
    {
        protected ShipAIControls m_ShipAiControls;
        protected EnemyDetector m_EnemyDetector;
        public override void OnStart()
        {
            m_ShipAiControls = Actor.GetComponent<ShipAIControls>();
            m_EnemyDetector = Actor.GetComponent<EnemyDetector>();
            base.OnStart();
        }

        protected T BlackboardGet<T>(BB_KEY key)
        {
            return Blackboard.Get<T>(key.ToString());
        }
        
        protected void BlackboardSet(BB_KEY key, object value)
        {
            Blackboard.Set(key.ToString(), value);
        }
    }
}