using AI;
using Bonsai.Core;

namespace BonsaiAI.Tasks
{
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
    }
}