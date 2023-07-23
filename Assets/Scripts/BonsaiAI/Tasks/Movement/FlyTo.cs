using System;
using System.Text;
using AI;
using Bonsai;
using BonsaiAI.Tasks;
using UnityEngine;


public enum FLY_TO_TYPE
{
    ENEMY,
    BLACKBOARD_TRANSFORM,
    BLACKBOARD_VECTOR3,
}

[BonsaiNode("Tasks/", "Arrow")]
public class FlyTo : Bonsai.Core.Task
{
    [SerializeField] private FLY_TO_TYPE type;
    [SerializeField] private bool away;
    [SerializeField] private BB_KEY bbKeyName = BB_KEY.MOVE_TARGET;
    [Range(0.1f, 1f)] private float throttleCutoff = 1f;
    
    private float atThreshold = 10;
    private EnemyDetector _mEnemyDetector;
    private ShipAIControls ai;

    private Vector3 targetPoint;

    public override void OnStart()
    {
        _mEnemyDetector = Actor.GetComponent<EnemyDetector>();
        ai = Actor.GetComponent<ShipAIControls>();
    }

    public override void Description(StringBuilder builder)
    {
        builder.Append($"{(away ? "from " : "")}{type.ToString()}");
    }

    public bool At(Vector3 point)
    {
        return Vector3.Distance(ai.transform.position, point) < atThreshold && !ai.IsMoving();
    }

    private void TargetSelection()
    {
        switch (type)
        {
            case FLY_TO_TYPE.ENEMY:
                EnemyTargetSelection();
                return;
            case FLY_TO_TYPE.BLACKBOARD_TRANSFORM:
            case FLY_TO_TYPE.BLACKBOARD_VECTOR3:
                KeyTargetSelection();
                return;
            default:
                throw new Exception($"No case for {type}");
        }
    }

    private void EnemyTargetSelection()
    {
        targetPoint = Vector3.zero;
        var enemy = _mEnemyDetector.Target;
        if (!enemy)
            return;
        targetPoint = _mEnemyDetector.Target.transform.position;
    }

    private void KeyTargetSelection()
    {
        targetPoint = Vector3.zero;

        if (type == FLY_TO_TYPE.BLACKBOARD_TRANSFORM)
        {
            Transform bbTransform = Blackboard.Get<Transform>(bbKeyName.ToString());
            if (bbTransform != null)
            {
                targetPoint = bbTransform.position;
                return;
            }
        }
        
        if (type == FLY_TO_TYPE.BLACKBOARD_VECTOR3)
        {
            targetPoint = Blackboard.Get<Vector3>(bbKeyName.ToString());
            return;
        }

        throw new Exception($"Unknown type for key selection {type}");
    }

    public override Status Run()
    {
        TargetSelection();

        if (targetPoint == default)
        {
            Debug.Log($"Target {bbKeyName} is default");
            return Status.Failure;
        }

        if (At(targetPoint))
        {
            return Status.Success;
        }

        ai.thisShip.MovementService.LimitThrottle(throttleCutoff);
        
        if (away)
        {
            ai.MoveAt(Actor.transform.position + (Actor.transform.position - targetPoint));
        }
        else
        {
            ai.MoveAt(targetPoint);
        }

        return Status.Running;
    }
}