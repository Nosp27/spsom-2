using System.Collections;
using System.Collections.Generic;
using AI;
using Bonsai;
using UnityEngine;

[BonsaiNode("Tasks/", "Arrow")]
public class PatrolPoints : Bonsai.Core.Task
{
    private Transform[] m_PatrolLocations;
    private int nextLocationIdx = 0;
    private Transform currentTarget => nextLocationIdx < m_PatrolLocations.Length ? m_PatrolLocations[nextLocationIdx] : null;

    private ShipAIControls m_ActorAIControls;

    public override void OnEnter()
    {
        if (m_ActorAIControls == null)
        {
            m_ActorAIControls = Actor.GetComponent<ShipAIControls>();
        }
    }

    public override Status Run()
    {
        if (m_PatrolLocations == null)
        {
            m_PatrolLocations = Tree.blackboard.Get<Transform[]>("PATROL_LOCATIONS");
        }

        if (currentTarget == null)
            return Status.Failure;
        
        if (Vector3.Distance(Actor.transform.position, currentTarget.position) < 10f)
        {
            nextLocationIdx += 1;
            return Status.Success;
        }
        m_ActorAIControls.MoveAt(currentTarget.position);
        return Status.Running;
    }
}
