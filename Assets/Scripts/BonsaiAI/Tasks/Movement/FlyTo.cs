using System;
using System.Text;
using AI;
using Bonsai;
using BonsaiAI;
using UnityEngine;


[BonsaiNode("Tasks/", "Arrow")]
public class FlyTo : Bonsai.Core.Task
{
    [SerializeField] private BBKey target;

    [SerializeField] private HEADING_MODE headingMode = HEADING_MODE.LOCKED_HEADING;
    [Range(0.1f, 1f)] private float throttleCutoff = 1f;

    [SerializeField] private bool away;
    [SerializeField] private bool waitForStop = true;
    [SerializeField] private float atThreshold = 10;

    [Tooltip(
        "Sometimes you want to stop near a target, but not at its position completely. " +
        "For example, if you fly exactly into target, you will collide with it"
    )]
    [SerializeField]
    private float flyByDistance = 0;

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
        builder.Append($"{(away ? "from " : "")}{target}");
    }

    public bool At(Vector3 point)
    {
        return Vector3.Distance(ai.transform.position, point) < atThreshold && !(ai.IsMoving() && waitForStop);
    }

    private void TargetSelection()
    {
        targetPoint = Utils.Position(Blackboard.Get(target));

        if (flyByDistance > 0)
            targetPoint += (Actor.transform.position - targetPoint).normalized * flyByDistance;
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