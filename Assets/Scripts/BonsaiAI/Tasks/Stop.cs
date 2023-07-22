using Bonsai;
using BonsaiAI.Tasks;

[BonsaiNode("Tasks/", "Arrow")]
public class Stop : AiShipTask
{
    public override Status Run()
    {
        if (m_ShipAiControls.thisShip.IsMoving())
            m_ShipAiControls.thisShip.CancelMovement();
        return Status.Success;
    }
}
