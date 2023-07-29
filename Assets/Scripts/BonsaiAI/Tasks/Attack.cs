using Bonsai;
using UnityEngine;

namespace BonsaiAI.Tasks
{
    [BonsaiNode("Tasks/", "Arrow")]
    public class Attack : AiShipTask
    {
        [SerializeField] private BBKey key;

        public override Status Run()
        {
            Transform target = Blackboard.Get<Transform>(key);
            if (target == null || m_ShipAiControls == null)
            {
                return Status.Failure;
            }
        
            m_ShipAiControls.thisShip.MovementService.TurnAt(target.transform.position);
            m_ShipAiControls.thisShip.Aim(target.transform.position);
            m_ShipAiControls.thisShip.Fire(target.transform.position);

            return Status.Running;
        }
    }
}
