using System.Collections;
using AI.States;
using GameControl.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class FighterAIStateMachine : MonoBehaviour
    {
        [SerializeField] private BaseShipAIState noopState;
        [SerializeField] private BaseShipAIState randomFlightState;
        [SerializeField] private BaseShipAIState chaseState;
        [SerializeField] private BaseShipAIState retreatState;
        [SerializeField] private BaseShipAIState flyAwayState;
        [SerializeField] private BaseShipAIState attackState;
        [SerializeField] private BaseShipAIState smallDodgeState;

        [SerializeField] private float attackRange = 150;
        [SerializeField] private float approachRange = 70;
        [SerializeField] private float tooNearDistance = 24;
        [SerializeField] private float dodgeChance = 0.2f;

        private StateMachine m_MovementStateMachine;
        private StateMachine m_AttackStateMachine;
        
        
        private ShipDamageModel m_DamageModel;
        private IEnemyDetector m_EnemyDetector;
        private bool allowDodgeTransition;

        private DamageModel enemy;
        Vector3 enemyLookVector;
        float enemyDistance;

        private void Start()
        {
            m_EnemyDetector = GetComponentInParent<IEnemyDetector>();
            m_DamageModel = GetComponentInParent<ShipDamageModel>();

            m_MovementStateMachine = BuildMovementStateMachine();
            m_AttackStateMachine = BuildAttackStateMachine();
            StartCoroutine(DodgeChanceCoroutine());
        }

        private void Update()
        {
            SetTriggers();
            m_MovementStateMachine.Tick();
            m_AttackStateMachine.Tick();
        }

        void SetTriggers()
        {
            enemy = m_EnemyDetector.Enemy;
            if (enemy)
            {
                enemyLookVector = enemy.transform.position - transform.position;
                enemyDistance = enemyLookVector.magnitude;
            }
        }

        StateMachine BuildMovementStateMachine()
        {
            StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
            
            // Default: FLy randomly if no enemies, retreat if enemy and lowhp
            sm.AddAnyTransition(randomFlightState, () => !EnemyInBounds());
            sm.AddAnyTransition(retreatState, () => LowHP() && EnemyInDangerDistance());

            // Spotted enemy - chase it
            sm.AddTransition(randomFlightState, chaseState, EnemyInBounds);

            // Enter attack range - do approach for combat position. After approach is done - attack
            sm.AddTransition(chaseState, noopState, () => EnemyInApproachRange() && !TooNear());
            sm.AddTransition(noopState, chaseState, () => !EnemyInAttackRange());
            sm.AddTransition(flyAwayState, noopState, () => EnemyInApproachRange() && !TooNear());

            // Fly away if enemy is too near
            sm.AddTransition(noopState, flyAwayState, TooNear);
            sm.AddTransition(flyAwayState, chaseState, () => !EnemyInAttackRange());
            
            // Dodge sometimes
            sm.AddTransition(noopState, smallDodgeState, ShouldDodge);
            sm.AddTransition(smallDodgeState, noopState, () => true);

            return sm;
        }

        StateMachine BuildAttackStateMachine()
        {
            StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
            sm.AddTransition(noopState, attackState, () => EnemyInAttackRange());
            sm.AddTransition(attackState, noopState, () => !EnemyInAttackRange());
            return sm;
        }


        bool EnemyInBounds() => enemy != null;

        bool LowHP() => m_DamageModel.Health < m_DamageModel.MaxHealth / 2;

        bool EnemyInDangerDistance() => EnemyInBounds() && enemyDistance < attackRange * 1.5f;
        bool EnemyInAttackRange() => EnemyInBounds() && enemyDistance < attackRange;
        bool EnemyInApproachRange() => EnemyInBounds() && enemyDistance < approachRange;
        bool TooNear() => enemyDistance < tooNearDistance;

        bool ShouldDodge()
        {
            if (allowDodgeTransition)
            {
                print("Dodge fire and reset");
                allowDodgeTransition = false;
                return true;
            }

            return false;
        }

        IEnumerator DodgeChanceCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (!allowDodgeTransition && Random.value < dodgeChance)
                {
                    allowDodgeTransition = true;
                }
            }
        }
    }
}