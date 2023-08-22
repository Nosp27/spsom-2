using Bonsai;
using Bonsai.Core;
using BonsaiAI;
using UnityEngine;


[BonsaiNode("Captures/", "Arrow")]
public class CapturePosition : Task
{
    [SerializeField] private BBKey outputKey;


    public override Status Run()
    {
        Blackboard.Set(outputKey, Actor.transform.position);
        return Status.Success;
    }
}
