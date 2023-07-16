
using System;
using System.Text;
using AI;
using Bonsai;
using UnityEngine;

public enum ENEMY_CONDITION_TYPE
{
    ENEMY_DETECTED,
    ENEMY_CLOSE,
    ENEMY_MID,
    ENEMY_FAR,
}

[BonsaiNode("Conditional/", "Question")]
public class EnemyCondition : Bonsai.Core.ConditionalTask
{
    [SerializeField] private ENEMY_CONDITION_TYPE m_ConditionType;
    private float midBound = 150;
    private float closeBound = 70;
    private EnemyDetector detector;
    private DamageModel enemy => detector.Target;
    private float m_EnemyDistance => Vector3.Distance(Actor.transform.position, enemy.transform.position);

    public override void OnStart()
    {
        detector = Actor.GetComponent<EnemyDetector>();
    }
    
    public override bool Condition()
    {
        if (detector == null)
        {
            throw new Exception($"No detector for {Actor.name}");
        }

        if (enemy == null)
        {
            return false;
        }

        if (m_ConditionType == ENEMY_CONDITION_TYPE.ENEMY_DETECTED || m_ConditionType == ENEMY_CONDITION_TYPE.ENEMY_FAR)
        {
            return true;
        }

        if (m_ConditionType == ENEMY_CONDITION_TYPE.ENEMY_CLOSE)
        {
            return m_EnemyDistance < closeBound;
        }

        if (m_ConditionType == ENEMY_CONDITION_TYPE.ENEMY_MID)
        {
            return m_EnemyDistance < midBound;
        }

        throw new Exception($"No processor for type {m_ConditionType}");
    }

    public override void Description(StringBuilder builder)
    {
        builder.Append(m_ConditionType.ToString());
    }
}
