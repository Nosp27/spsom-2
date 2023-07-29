using Bonsai;
using BonsaiAI.Tasks;
using UnityEngine;

[BonsaiNode("Tasks/", "Arrow")]
public class Streif : AiShipTask
{
    [SerializeField] private Vector3 localOffset = Vector3.left;
    [SerializeField] private float throttleCutoff = 0.3f;

    private Ship thisShip;
    
    public override void OnEnter()
    {
        thisShip = m_ShipAiControls.thisShip;
        thisShip.MovementService.ChangeHeadingMode(HEADING_MODE.FREE_HEADING);
        thisShip.MovementService.LimitThrottle(throttleCutoff);
        base.OnEnter();
    }

    public override Status Run()
    {
        thisShip.MovementService.MoveAtDirection(
            thisShip.transform.position + thisShip.transform.InverseTransformPoint(localOffset)
        );
        return Status.Running;
    }
    
    public override void OnExit()
    {
        thisShip.MovementService.ChangeHeadingMode(HEADING_MODE.LOCKED_HEADING);
        thisShip.MovementService.LimitThrottle(1);
        base.OnExit();
    }
}