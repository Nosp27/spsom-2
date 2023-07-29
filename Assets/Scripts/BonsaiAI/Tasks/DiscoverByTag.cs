using BonsaiAI.Tasks;
using UnityEngine;

[Bonsai.BonsaiNode("Tasks/", "Arrow")]
public class DiscoverByTag : AiShipTask
{
    [SerializeField] private string tag;
    [SerializeField] private BB_KEY targetKey = BB_KEY.MOVE_TARGET;

    public override Status Run()
    {
        Transform target = GameObject.FindWithTag(tag)?.transform;
        if (target == null)
            return Status.Failure;
        BlackboardSet(targetKey, target);
        return Status.Success;
    }
}
