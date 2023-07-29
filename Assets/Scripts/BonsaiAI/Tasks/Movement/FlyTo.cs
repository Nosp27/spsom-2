using System;
using System.Text;
using AI;
using Bonsai;
using BonsaiAI;
using UnityEngine;


public enum FLY_TO_TYPE
{
    BLACKBOARD_TRANSFORM,
    BLACKBOARD_VECTOR3,
}

[BonsaiNode("Tasks/", "Arrow")]
public class FlyTo : Bonsai.Core.Task
{
    [SerializeField] private FLY_TO_TYPE type;
    
    [SerializeField] private BBKey target;

    [SerializeField] private HEADING_MODE headingMode = HEADING_MODE.LOCKED_HEADING;
    [Range(0.1f, 1f)] private float throttleCutoff = 1f;
    
    [SerializeField] private bool away;
    [SerializeField] private bool waitForStop = true;
    [SerializeField] private float atThreshold = 10;
    
    private ShipAIControls ai;
    private Vector3 targetPoint;

    public override void OnStart()
    {
        ai = Actor.GetComponent<ShipAIControls>();
    }

    public override void OnExit()
    {
        ai.thisShip.MovementService.ChangeHeadingMode(HEADING_MODE.LOCKED_HEADING);
        base.OnExit();
    }

    public override void Description(StringBuilder builder)
    {
        builder.Append($"{(away ? "from " : "")}{type.ToString()}");
    }

    public bool At(Vector3 point)
    {
        return Vector3.Distance(ai.transform.position, point) < atThreshold && !(ai.IsMoving() && waitForStop);
    }

    private void TargetSelection()
    {
        targetPoint = Vector3.zero;

        if (type == FLY_TO_TYPE.BLACKBOARD_TRANSFORM)
        {
            Transform bbTransform = Blackboard.Get<Transform>(target);
            if (bbTransform != null)
            {
                targetPoint = bbTransform.position;
                return;
            }
        }
        
        if (type == FLY_TO_TYPE.BLACKBOARD_VECTOR3)
        {
            targetPoint = Blackboard.Get<Vector3>(target);
            return;
        }

        throw new Exception($"Unknown type for key selection {type}");
    }

    public override Status Run()
    {
        TargetSelection();

        if (targetPoint == default)
        {
            Debug.Log($"Target {target} is default");
            return Status.Failure;
        }

        if (At(targetPoint))
        {
            return Status.Success;
        }

        ai.thisShip.MovementService.ChangeHeadingMode(headingMode);
        ai.thisShip.MovementService.LimitThrottle(throttleCutoff);
        
        if (away)
        {
            ai.MoveAt(Actor.transform.position + (Actor.transform.position - targetPoint));
        }
        else
        {
            Debug.DrawLine(Actor.transform.position, targetPoint, Color.yellow);
            ai.MoveAt(targetPoint);
        }

        return Status.Running;
    }
}