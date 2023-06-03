using System.Collections;
using AI.States;
using GameControl.StateMachine;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI
{
    public class DiverStateMachine : MonoBehaviour
    {
        [SerializeField] private BaseShipAIState noopState;
        [SerializeField] private BaseShipAIState randomFlightState;
        [SerializeField] private BaseShipAIState chaseState;
        [SerializeField] private BaseShipAIState flyAlongState;
        [SerializeField] private BaseShipAIState diveBackState;
        [SerializeField] private BaseShipAIState attackState;

        [SerializeField] private float attackRange = 50;
        [SerializeField] private float tooNearDistance = 24;
        [SerializeField] float minSafeDistance = 70;

        private StateMachine m_MovementStateMachine;
        private StateMachine m_AttackStateMachine;
        
        private IEnemyDetector m_EnemyDetector;
        private bool allowDodgeTransition;

        private DamageModel enemy;
        Vector3 enemyLookVector;
        float enemyDistance;

        private void Start()
        {
            m_EnemyDetector = GetComponentInParent<IEnemyDetector>();

            m_MovementStateMachine = BuildMovementStateMachine();
            m_AttackStateMachine = BuildAttackStateMachine();
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
            
            // Default: FLy idle if no enemies
            sm.AddAnyTransition(randomFlightState, () => !EnemyInBounds());

            // Spotted enemy - chase it (dive in)
            sm.AddTransition(randomFlightState, chaseState, EnemyInBounds);

            // If enemy is too near - dive back until enemy in safe range, then fly along for several seconds
            sm.AddTransition(chaseState, diveBackState, TooNear);
            sm.AddTransition(diveBackState, flyAlongState, EnemyInSafeRange);
            sm.AddTransition(flyAlongState, chaseState, () => true);

            return sm;
        }

        StateMachine BuildAttackStateMachine()
        {
            StateMachine sm = new StateMachine(AnyStatePriority.ANY_STATES_FIRST);
            sm.AddTransition(noopState, attackState, EnemyInAttackRange);
            sm.AddTransition(attackState, noopState, () => !EnemyInAttackRange());
            return sm;
        }

        bool EnemyInBounds() => enemy != null;

        bool EnemyInSafeRange() => EnemyInBounds() && enemyDistance > minSafeDistance;
        bool EnemyInAttackRange() => EnemyInBounds() && enemyDistance < attackRange;
        bool TooNear() => enemyDistance < tooNearDistance;
    }
}