using Bonsai;
using UnityEngine;

namespace BonsaiAI.Tasks.Movement
{
    [BonsaiNode("Tasks/", "Arrow")]
    public class TurnAt : AiShipTask
    {
        [SerializeField] private BBKey targetTransformKey;
        [SerializeField] private float angleTolerance = 5;

        [ShowAtRuntime] private float angle;

        public override Status Run()
        {
            Transform targetTransform = Blackboard.Get<Transform>(targetTransformKey);

            Transform actorTransform = Actor.transform;
            angle = Vector3.Angle(
                actorTransform.forward,
                Vector3.ProjectOnPlane(
                    targetTransform.position - actorTransform.position, actorTransform.up
                )
            );
            if (angle < angleTolerance)
            {
                return Status.Success;
            }

            m_ShipAiControls.thisShip.MovementService.TurnAt(targetTransform.position);
            return Status.Running;
        }
    }
}