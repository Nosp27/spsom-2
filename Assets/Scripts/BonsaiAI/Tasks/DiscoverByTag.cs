using BonsaiAI;
using BonsaiAI.Tasks;
using UnityEngine;

[Bonsai.BonsaiNode("Tasks/", "Arrow")]
public class DiscoverByTag : AiShipTask
{
    [SerializeField] private string tag;
    [SerializeField] private BBKey outputKey;

    public override Status Run()
    {
        Transform target = GameObject.FindWithTag(tag)?.transform;
        if (target == null)
            return Status.Failure;
        Blackboard.Set(outputKey, target);
        return Status.Success;
    }
}
