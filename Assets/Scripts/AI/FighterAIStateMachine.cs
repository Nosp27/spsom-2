using System.Collections;
using AI.States;
using GameControl.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class FighterAIStateMachine : MonoBehaviour
    {
        [SerializeField] private BaseShipAIState TestCurrentState;
        [SerializeField] private bool TestSD;
        [SerializeField] private string TestTransitionDescription;

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

        private StateMachine m_StateMachine;
        private ShipDamageModel m_DamageModel;
        private EnemyDetector m_EnemyDetector;
        private bool allowDodgeTransition;

        private Ship enemy;
        Vector3 enemyLookVector;
        float enemyDistance;

        private void Start()
        {
            m_EnemyDetector = GetComponentInParent<EnemyDetector>();
            m_DamageModel = GetComponentInParent<ShipDamageModel>();

            m_StateMachine = BuildStateMachine();
            StartCoroutine(DodgeChanceCoroutine());
        }

        private void Update()
        {
            TestCurrentState = (BaseShipAIState) m_StateMachine.CurrentState;
            TestTransitionDescription = m_StateMachine.firedTransition;

            SetTriggers();
            m_StateMachine.Tick();
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

        StateMachine BuildStateMachine()
        {
            StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
            
            // Default: FLy randomly if no enemies, retreat if enemy and lowhp
            sm.AddAnyTransition(randomFlightState, () => !EnemyInBounds());
            sm.AddAnyTransition(retreatState, () => LowHP() && EnemyInDangerDistance());

            // Spotted enemy - chase it
            sm.AddTransition(randomFlightState, chaseState, EnemyInBounds);

            // Enter attack range - do approach for combat position. After approach is done - attack
            sm.AddTransition(chaseState, flyAwayState, TooNear);
            sm.AddTransition(chaseState, attackState, () => EnemyInApproachRange() && !TooNear());
            sm.AddTransition(flyAwayState, attackState, () => EnemyInApproachRange() && !TooNear());
            
            // Fly away if enemy is too near
            sm.AddTransition(attackState, flyAwayState, TooNear);
            sm.AddTransition(flyAwayState, chaseState, () => !EnemyInAttackRange());
            
            // Dodge sometimes
            sm.AddTransition(attackState, smallDodgeState, ShouldDodge);
            sm.AddTransition(smallDodgeState, attackState, () => true);

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