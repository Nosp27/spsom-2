using Bonsai;
using BonsaiAI.Tasks;

[BonsaiNode("Tasks/", "Arrow")]
public class Stop : AiShipTask
{
    public override Status Run()
    {
        if (m_ShipAiControls.thisShip.MovementService.IsMoving())
            m_ShipAiControls.thisShip.MovementService.CancelMovement();
        return Status.Success;
    }
}
