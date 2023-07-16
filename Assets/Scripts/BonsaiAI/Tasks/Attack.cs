using AI;
using Bonsai;
using UnityEngine;

[BonsaiNode("Tasks/", "Arrow")]
public class Attack : Bonsai.Core.Task
{
    [SerializeField] private float AttachRange = 60;
    private EnemyDetector _mEnemyDetector;
    private DamageModel enemy => _mEnemyDetector.Target;
    private Ship thisShip => ai.thisShip;
    private ShipAIControls ai;

    public override void OnStart()
    {
        _mEnemyDetector = Actor.GetComponent<EnemyDetector>();
        ai = Actor.GetComponent<ShipAIControls>();
    }

    public override Status Run()
    {
        if (enemy == null || !ai || Vector3.Distance(enemy.transform.position, Actor.transform.position) > AttachRange)
        {
            return Status.Failure;
        }

        thisShip.Track(enemy.transform);
        thisShip.TurnOnPlace(enemy.transform.position);
        if (thisShip.Aimed())
        {
            thisShip.Fire(enemy.transform.position);
        }
        return Status.Running;
    }
}