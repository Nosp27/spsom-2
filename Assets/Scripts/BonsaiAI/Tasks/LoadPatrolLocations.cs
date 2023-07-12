using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using Bonsai;
using UnityEngine;

[BonsaiNode("Tasks/", "Arrow")]
public class LoadPatrolLocations : Bonsai.Core.Task
{
    public override Status Run()
    {
        Transform[] locs = Tree.blackboard.Get<Transform[]>("PATROL_LOCATIONS");
        
        if (locs != null && locs.Length > 0)
            return Status.Success;
        
        locs = GameObject.FindGameObjectsWithTag("Waypoint").Select(x => x.transform).ToArray();
        if (locs.Length == 0)
            return Status.Failure;
        
        Tree.blackboard.Set("PATROL_LOCATIONS", locs);
        return Status.Success;
    }
}