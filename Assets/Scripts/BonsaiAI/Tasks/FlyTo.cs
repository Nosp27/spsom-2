using System.Text;
using AI;
using Bonsai;
using UnityEngine;


public enum FLY_TO_TYPE
{
    ENEMY,
}

[BonsaiNode("Tasks/", "Arrow")]
public class FlyTo : Bonsai.Core.Task
{
    [SerializeField] private FLY_TO_TYPE type;
    [SerializeField] private bool away;
    
    private float atThreshold = 10;
    private EnemyDetector _mEnemyDetector;
    private ShipAIControls ai;
    
    private Transform target;
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
        if (type == FLY_TO_TYPE.ENEMY)
        {
            var enemy = _mEnemyDetector.Target;
            if (enemy)
            {
                target = _mEnemyDetector.Target.transform;
                targetPoint = target.transform.position;
            }
            else
            {
                target = null;
                targetPoint = Vector3.zero;
            }
            return;
        }
    }

    public override Status Run()
    {
        Debug.Log("Run");
        TargetSelection();
        
        if (targetPoint == Vector3.zero)
        {
            return Status.Failure;
        }

        if (At(targetPoint))
        {
            return Status.Success;
        }

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